import { Component, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AppStateService } from '../../services/app-state.service';
import { TourService } from '../../services/tour.service';
import { NotificationService } from '../../services/notification.service';
import { Tour } from '../../models/tour';
import { Transport } from '../../models/transport';
import { Mapview } from '../mapview/mapview';
import { TourLogFormComponent } from '../tour-log-form/tour-log-form.component';
import { TourLog } from '../../models/tourLog';

@Component({
  selector: 'app-tour-detail',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, Mapview, TourLogFormComponent],
  templateUrl: './tour-detail.html',
  styleUrl: './tour-detail.scss',
})
export class TourDetail implements OnInit {
  private readonly route = inject(ActivatedRoute);
  private readonly router = inject(Router);
  private readonly tourService = inject(TourService);
  private readonly notifications = inject(NotificationService);
  private readonly fb = inject(FormBuilder);
  private readonly appState = inject(AppStateService);

  readonly tour = signal<Tour | null>(null);
  readonly isEditing = signal(false);
  readonly showTourLogForm = signal(false);
  readonly editingLog = signal<TourLog | null>(null);
  readonly searchQuery = signal('');
  readonly isSubmitting = signal(false);
  readonly showDeleteConfirm = signal(false);
  readonly deleteTargetType = signal<'tour' | 'log' | null>(null);
  readonly deleteTargetGuid = signal<string | null>(null);
  readonly transportTypes = Object.values(Transport);

  editForm: FormGroup;

  constructor() {
    this.editForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      from: ['', [Validators.required]],
      to: ['', [Validators.required]],
      transportType: [Transport.Car, [Validators.required]],
      rating: [0, [Validators.required, Validators.min(0), Validators.max(5)]],
    });
  }

  ngOnInit(): void {
    this.loadTour();
  }

  private loadTour(): void {
    const tourGuid = this.route.snapshot.paramMap.get('tourGuid');
    if (!tourGuid) {
      this.router.navigate(['/']);
      return;
    }

    const tour = this.tourService.getTourByGuid(tourGuid);
    if (!tour) {
      this.notifications.error('Tour not found');
      this.router.navigate(['/']);
      return;
    }

    const currentUser = this.appState.currentUser();
    if (tour.userGuid !== currentUser?.userGuid) {
      this.notifications.error('You do not have permission to view this tour');
      this.router.navigate(['/']);
      return;
    }

    this.tour.set(tour);
    this.editForm.patchValue(tour);
  }

  toggleEdit(): void {
    this.isEditing.update((val) => !val);
    if (!this.isEditing()) {
      const tour = this.tour();
      if (tour) {
        this.editForm.patchValue(tour);
      }
    }
  }

  async onSave(): Promise<void> {
    if (this.editForm.invalid) {
      this.notifications.error('Please fix the form errors');
      return;
    }

    const currentTour = this.tour();
    if (!currentTour) return;

    this.isSubmitting.set(true);
    try {
      const updatedTour = await this.tourService.updateTour(currentTour.tourGuid, this.editForm.value);
      if (updatedTour) {
        this.tour.set(updatedTour);
      }
      this.isEditing.set(false);
      this.notifications.success('Tour updated successfully!');
    } catch (err: any) {
      this.notifications.error(err.message || 'Failed to update tour');
    } finally {
      this.isSubmitting.set(false);
    }
  }

  async onDelete(): Promise<void> {
    const currentTour = this.tour();
    if (!currentTour) return;

    this.deleteTargetType.set('tour');
    this.deleteTargetGuid.set(currentTour.tourGuid);
    this.showDeleteConfirm.set(true);
  }

  onDeleteTourLog(tourLogGuid: string): void {
    this.deleteTargetType.set('log');
    this.deleteTargetGuid.set(tourLogGuid);
    this.showDeleteConfirm.set(true);
  }

  async confirmDelete(): Promise<void> {
    const type = this.deleteTargetType();
    const guid = this.deleteTargetGuid();

    if (!type || !guid) return;

    this.isSubmitting.set(true);
    try {
      if (type === 'tour') {
        this.tourService.deleteTour(guid);
        this.notifications.success('Tour deleted successfully!');
        await this.router.navigate(['/']);
      } else if (type === 'log') {
        const currentTour = this.tour();
        if (currentTour) {
          this.tourService.deleteTourLog(currentTour.tourGuid, guid);
          const updatedTour = this.tourService.getTourByGuid(currentTour.tourGuid);
          if (updatedTour) {
            this.tour.set(updatedTour);
          }
          this.notifications.success('Tour log deleted successfully!');
        }
      }
    } catch (err: any) {
      this.notifications.error(err.message || 'Failed to delete');
    } finally {
      this.isSubmitting.set(false);
      this.showDeleteConfirm.set(false);
      this.deleteTargetType.set(null);
      this.deleteTargetGuid.set(null);
    }
  }

  cancelDelete(): void {
    this.showDeleteConfirm.set(false);
    this.deleteTargetType.set(null);
    this.deleteTargetGuid.set(null);
  }

  onTourLogCreated(tourLog: TourLog): void {
    const currentTour = this.tour();
    if (!currentTour) return;

    try {
      this.tourService.addTourLog(currentTour.tourGuid, tourLog);
      const updatedTour = this.tourService.getTourByGuid(currentTour.tourGuid);
      if (updatedTour) {
        this.tour.set(updatedTour);
      }
      this.showTourLogForm.set(false);
      this.notifications.success('Tour log added successfully!');
    } catch (err: any) {
      this.notifications.error(err.message || 'Failed to add tour log');
    }
  }

  onEditTourLog(log: TourLog): void {
    this.editingLog.set(log);
    this.showTourLogForm.set(true);
  }

  onTourLogUpdated(updatedLog: TourLog): void {
    const currentTour = this.tour();
    if (!currentTour) return;

    try {
      this.tourService.updateTourLog(currentTour.tourGuid, updatedLog);
      const updatedTour = this.tourService.getTourByGuid(currentTour.tourGuid);
      if (updatedTour) {
        this.tour.set(updatedTour);
      }
      this.editingLog.set(null);
      this.showTourLogForm.set(false);
      this.notifications.success('Tour log updated successfully!');
    } catch (err: any) {
      this.notifications.error(err.message || 'Failed to update tour log');
    }
  }

  getFilteredLogs(): TourLog[] {
    const logs = this.tour()?.tourLogs ?? [];
    const q = this.searchQuery().trim().toLowerCase();
    if (!q) return logs;
    return logs.filter((l) => {
      const dateStr = new Date(l.dateTime).toLocaleString().toLowerCase();
      const comment = (l.comment || '').toLowerCase();
      const rating = String(l.rating);
      return dateStr.includes(q) || comment.includes(q) || rating.includes(q);
    });
  }

  goBack(): void {
    this.router.navigate(['/']);
  }

  getErrorMessage(controlName: string): string {
    const control = this.editForm.get(controlName);
    if (!control?.errors || !control?.touched) return '';

    if (control.errors['required']) return `${controlName} is required`;
    if (control.errors['minlength'])
      return `${controlName} must be at least ${control.errors['minlength'].requiredLength} characters`;
    if (control.errors['min'])
      return `${controlName} must be greater than ${control.errors['min'].min}`;
    if (control.errors['max'])
      return `${controlName} must be less than or equal to ${control.errors['max'].max}`;

    return 'Invalid input';
  }
}
