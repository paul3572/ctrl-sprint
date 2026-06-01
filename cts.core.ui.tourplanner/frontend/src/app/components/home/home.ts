import { Component, computed, effect, inject, signal } from '@angular/core';
import { Router } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

import { AppStateService } from '../../services/app-state.service';
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
  private readonly notifications = inject(NotificationService);

  // Local state (searchQuery is writable for two-way binding)
  readonly searchQuery = signal('');
  readonly loading = signal(false);
  readonly tours = signal<Tour[]>([]);
  readonly showCreateModal = signal(false);

  // Derived
  readonly filteredTours = computed(() => {
    const q = this.searchQuery().trim().toLowerCase();
    if (!q) return this.tours();
    return this.tours().filter(
      (t) => t.name.toLowerCase().includes(q) || (t.description ?? '').toLowerCase().includes(q),
    );
  });

  readonly totalTours = computed(() => this.tours().length);
  readonly totalLogs = computed(() =>
    this.tours().reduce((sum, t) => sum + (t.tourLogs.length ?? 0), 0),
  );
  readonly mostPopularTour = computed(() => {
    const tours = this.tours();
    if (tours.length === 0) return null;
    return tours.reduce((a, b) => ((a.rating ?? 0) > (b.rating ?? 0) ? a : b)).name;
  });

  constructor() {
    this.loadInitialTours();

    // React to search query changes for side effects if needed
    effect(() => {
      this.searchQuery();
      // You could add side effects here like analytics, etc.
    });
  }

  async loadInitialTours() {
    this.loading.set(true);
    try {
      // Mock tours (replace with TourService call)
      const mock: Tour[] = [
        {
          tourGuid: '00000000-0000-0000-0000-000000000000',
          name: 'Danube River Bike Tour',
          description: 'Scenic bike ride along the Danube.',
          from: 'Vienna City Center',
          to: 'Greifenstein',
          transportType: Transport.Bike,
          tourDistance: 42.5,
          estimatedTimeMinutes: 180,
          rating: 3,
          tourLogs: [],
        },
        {
          tourGuid: '00000000-0000-0000-0000-000000000001',
          name: 'Alpine Hike',
          description: 'Challenging hike with mountain views.',
          from: 'Karsee Lake',
          to: 'Zugspitze Peak',
          transportType: Transport.Foot,
          tourDistance: 16.2,
          estimatedTimeMinutes: 480,
          rating: 1,
          tourLogs: [],
        },
      ];
      await new Promise((res) => setTimeout(res, 300));
      this.tours.set(mock);
    } catch (err: any) {
      this.notifications.error('Failed to load tours');
    } finally {
      this.loading.set(false);
    }
  }


  onCreateTourClicked() {
    this.showCreateModal.set(true);
  }

  onTourCreated(tour: Tour) {
    this.tours.update((tours) => [tour, ...tours]);
    this.showCreateModal.set(false);
    this.notifications.success(`Tour "${tour.name}" created successfully!`);
  }

  onCreateModalClosed() {
    this.showCreateModal.set(false);
  }

  onSelectTour(tourGuid: string) {
    this.router.navigate(['/tours', tourGuid]);
  }

  async onLogoutClicked() {
    this.appState.logout();
    await this.router.navigate(['/login']);
  }

  protected readonly Transport = Transport;
}
