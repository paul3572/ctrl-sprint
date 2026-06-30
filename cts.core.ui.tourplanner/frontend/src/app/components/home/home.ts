import { Component, computed, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { AppStateService } from '../../services/app-state.service';
import { TourService } from '../../services/tour.service';
import { NotificationService } from '../../services/notification.service';
import { TourFormComponent } from '../tour-form/tour-form.component';
import type { Tour } from '../../models/tour';
import { Transport } from '../../models/transport';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule, TourFormComponent],
  templateUrl: './home.html',
  styleUrls: ['./home.scss'],
})
export class Home {
  private readonly router = inject(Router);
  private readonly appState = inject(AppStateService);
  private readonly tourService = inject(TourService);
  private readonly notifications = inject(NotificationService);

  // Local state
  readonly searchQuery = signal('');
  readonly loading = computed(() => this.tourService.isLoading());
  readonly showCreateModal = signal(false);

  // Derived
  readonly filteredTours = computed(() => {
    const q = this.searchQuery().trim().toLowerCase();
    const tours = this.tourService.userTours();
    if (!q) return tours;
    return tours.filter(
      (t) => t.name.toLowerCase().includes(q) || (t.description ?? '').toLowerCase().includes(q),
    );
  });

  readonly totalTours = computed(() => this.tourService.userTours().length);
  readonly totalLogs = computed(() =>
    this.tourService.userTours().reduce((sum, t) => sum + (t.tourLogs?.length ?? 0), 0),
  );
  readonly mostPopularTour = computed(() => {
    const tours = this.tourService.userTours();
    if (tours.length === 0) return null;
    return tours.reduce((a, b) => ((a.rating ?? 0) > (b.rating ?? 0) ? a : b)).name;
  });

  constructor() {
    effect(() => {
      this.searchQuery();
    });
  }

  async loadToursFromBackend() {
    try {
      await this.tourService.loadToursFromBackend();
    } catch (err: any) {
      this.notifications.error('Failed to load tours from backend');
    }
  }

  onCreateTourClicked() {
    this.showCreateModal.set(true);
  }

  async onTourCreated(tour: Tour) {
    try {
      const success = await this.tourService.addTour(tour);

      if (!success) {
        this.notifications.error("Something went wrong");
        return;
      }

      this.showCreateModal.set(false);
      this.notifications.success(`Tour "${tour.name}" created successfully!`);
    } catch (err: any) {
      this.notifications.error(err.message || 'Failed to create tour');
    }
  }

  onCreateModalClosed() {
    this.showCreateModal.set(false);
  }

  onSelectTour(tourGuid: string) {
    this.router.navigate(['/tour', tourGuid]);
  }

  async onLogoutClicked() {
    this.appState.logout();
    await this.router.navigate(['/login']);
  }

  protected readonly Transport = Transport;
}
