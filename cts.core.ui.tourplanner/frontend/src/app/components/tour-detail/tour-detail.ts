import { Component, effect, inject, OnInit, signal } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';

import { AppStateService } from '../../services/app-state.service';
import { TourService } from '../../services/tour.service';
import { NotificationService } from '../../services/notification.service';
import { Tour } from '../../models/tour';
import { Transport } from '../../models/transport';
import { TourLog } from '../../models/tourLog';
import { WeatherService } from '../../services/weather.service';
import { Mapview } from '../mapview/mapview';
import { TourLogFormComponent } from '../tour-log-form/tour-log-form.component';
import { Weather } from '../../contracts/Weather';
import { getWeatherEmoji } from '../../utils/weather.util';

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
  private readonly weatherService = inject(WeatherService);

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
  readonly weatherFrom = signal<Weather | null>(null);
  readonly weatherTo = signal<Weather | null>(null);

  editForm: FormGroup;

  constructor() {
    this.editForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]],
      description: [''],
      from: [{ value: '', disabled: true }, [Validators.required]],
      to: [{ value: '', disabled: true }, [Validators.required]],
      transportType: [Transport.Car, [Validators.required]],
      rating: [0, [Validators.required, Validators.min(0), Validators.max(5)]],
    });

    effect(() => {
      const current = this.tour();
      if (!current) return;

      const updated = this.tourService.getTourByGuid(current.tourGuid);
      if (updated && updated !== current) {
        this.tour.set(updated);
      }
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
    this.loadWeather(tour);
  }

  private async loadWeather(tour: Tour): Promise<void> {
    if (!tour.routeGeometry?.coordinates || tour.routeGeometry.coordinates.length === 0) {
      return;
    }

    const coordinates = tour.routeGeometry.coordinates;
    const firstCoord = coordinates[0];
    const lastCoord = coordinates[coordinates.length - 1];

    let fromLat: number | null = null;
    let fromLon: number | null = null;
    let toLat: number | null = null;
    let toLon: number | null = null;

    const isArrayFormat = (coord: any): coord is number[] => {
      return Array.isArray(coord);
    };

    if (isArrayFormat(firstCoord)) {
      fromLon = firstCoord[0];
      fromLat = firstCoord[1];
      toLon = (lastCoord as unknown as number[])[0];
      toLat = (lastCoord as unknown as number[])[1];
    } else {
      fromLat = firstCoord.latitude;
      fromLon = firstCoord.longitude;
      toLat = lastCoord.latitude;
      toLon = lastCoord.longitude;
    }

    if (fromLat !== null && fromLon !== null) {
      try {
        this.weatherFrom.set(await this.weatherService.getWeather(fromLat, fromLon));
      } catch (error) {
        console.error('Failed to fetch weather for starting point', error);
      }
    }

    if (toLat !== null && toLon !== null) {
      try {
        this.weatherTo.set(await this.weatherService.getWeather(toLat, toLon));
      } catch (error) {
        console.error('Failed to fetch weather for destination', error);
      }
    }
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
      const updatedTour = await this.tourService.updateTour(
        currentTour.tourGuid,
        this.editForm.value,
      );
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
        await this.tourService.deleteTour(guid);
        this.notifications.success('Tour deleted successfully!');
        await this.router.navigate(['/']);
      } else if (type === 'log') {
        const currentTour = this.tour();
        if (currentTour) {
          await this.tourService.deleteTourLog(currentTour.tourGuid, guid);
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
      const dateStr = new Date(l.timestamp).toLocaleString().toLowerCase();
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

  getPopularity(): string {
    const count = this.tour()?.tourLogs?.length ?? 0;

    if (count === 0) return 'Not popular 🥴';
    if (count < 5) return 'Low 📉';
    if (count < 15) return 'Medium ✨';
    return 'High 🔥';
  }

  getChildFriendliness(): number {
    const logs = this.tour()?.tourLogs ?? [];

    if (logs.length === 0) return 0;

    const avgDifficulty = logs.reduce((s, l) => s + l.difficulty, 0) / logs.length;

    const avgTime = logs.reduce((s, l) => s + l.totalTimeMin, 0) / logs.length;

    const avgDistance = logs.reduce((s, l) => s + l.totalDistanceInMeters, 0) / logs.length;

    let stars = 5;

    if (avgDifficulty > 4) stars--;
    if (avgDifficulty > 3) stars--;

    if (avgTime > 120) stars--;
    if (avgDistance > 10000) stars--;

    return Math.max(1, stars);
  }

  protected readonly getWeatherEmoji = getWeatherEmoji;
}
