import { computed, Injectable, signal } from '@angular/core';
import { Tour } from '../models/tour';
import { TourLog } from '../models/tourLog';
import { Transport } from '../models/transport';
import { AppStateService } from './app-state.service';
import {firstValueFrom} from 'rxjs';
import {HttpClient, HttpErrorResponse} from '@angular/common/http';

@Injectable({ providedIn: 'root' })
export class TourService {
  private static readonly TourStorageKey = 'tp_tours';
  private readonly _tours = signal<Tour[]>([]);
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);

  // All tours (from all users) - exposed for future "browse all tours" feature
  readonly allTours = computed(() => this._tours());

  // Computed: tours for current user only
  readonly userTours = computed(() => {
    const currentUser = this.appState.currentUser();
    if (!currentUser) return [];
    return this._tours().filter((t) => t.userGuid === currentUser.userGuid);
  });

  constructor(private readonly appState: AppStateService,
              private readonly http: HttpClient) {
    this.hydrateFromStorage();
  }

  async addTour(tour: Tour): Promise<void> {
    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot add tour: no authenticated user');
    }

    const newTour: Tour = {
      ...tour,
      userGuid: currentUser.userGuid,
    };

    try {
      const response = await firstValueFrom(
        this.http.post<string>(`/api/tour/${newTour.userGuid}`, {
          userGuid: newTour.userGuid,
          name: newTour.name,
          description: newTour.description,
          from: newTour.from,
          to: newTour.to,
          transportName: newTour.transportType,
          rating: newTour.rating,
        }),
      );

      this._tours.update((tours) => [newTour, ...tours]);
      this.persistToStorage();

    }
    catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  getTourByGuid(tourGuid: string): Tour | undefined {
    return this._tours().find((t) => t.tourGuid === tourGuid);
  }

  updateTour(tourGuid: string, updates: Partial<Tour>): void {
    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot update tour: no authenticated user');
    }

    this._tours.update((tours) =>
      tours.map((t) =>
        t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid ? { ...t, ...updates } : t,
      ),
    );
    this.persistToStorage();
  }

  deleteTour(tourGuid: string): void {
    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot delete tour: no authenticated user');
    }

    this._tours.update((tours) =>
      tours.filter((t) => !(t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid)),
    );
    this.persistToStorage();
  }

  addTourLog(tourGuid: string, tourLog: TourLog): void {
    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot add tour log: no authenticated user');
    }

    this._tours.update((tours) =>
      tours.map((t) => {
        if (t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid) {
          return {
            ...t,
            tourLogs: [tourLog, ...(t.tourLogs ?? [])],
          };
        }
        return t;
      }),
    );
    this.persistToStorage();
  }

  deleteTourLog(tourGuid: string, tourLogGuid: string): void {
    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot delete tour log: no authenticated user');
    }

    this._tours.update((tours) =>
      tours.map((t) => {
        if (t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid) {
          return {
            ...t,
            tourLogs: t.tourLogs?.filter((log) => log.tourLogGuid !== tourLogGuid) ?? [],
          };
        }
        return t;
      }),
    );
    this.persistToStorage();
  }

  updateTourLog(tourGuid: string, updatedLog: TourLog): void {
    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot update tour log: no authenticated user');
    }

    this._tours.update((tours) =>
      tours.map((t) => {
        if (t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid) {
          return {
            ...t,
            tourLogs: (t.tourLogs ?? []).map((log) => (log.tourLogGuid === updatedLog.tourLogGuid ? { ...log, ...updatedLog } : log)),
          };
        }
        return t;
      }),
    );
    this.persistToStorage();
  }

  /**
   * Load tours from mock backend API.
   * Later: replace with HttpClient.get('/api/tours')
   * Merges backend tours with locally created tours (by tourGuid).
   */
  async loadToursFromBackend(): Promise<void> {
    this.isLoading.set(true);
    try {
      // Simulate network delay
      await new Promise((res) => setTimeout(res, 500));

      // Mock backend response: all tours from all users
      const backendTours: Tour[] = this.getMockTourData();
      const existingTours = this._tours();

      // Merge: keep existing local tours + add backend tours not already present
      const guids = new Set(existingTours.map((t) => t.tourGuid));
      const newBackendTours = backendTours.filter((t) => !guids.has(t.tourGuid));
      const merged = [...existingTours, ...newBackendTours];

      this._tours.set(merged);
      this.persistToStorage();
    } catch (err) {
      console.error('[TourService] Failed to load tours from backend', err);
      throw err;
    } finally {
      this.isLoading.set(false);
    }
  }

  /**
   * Get mock tour data as if from backend API
   * Uses fixed UUIDs so deduplication works across reloads
   */
  private getMockTourData(): Tour[] {
    const currentUser = this.appState.currentUser();
    const currentUserGuid = currentUser?.userGuid || 'current-user';

    // Demo: current user's tours + tours from other users
    // Using fixed UUIDs so mock tours don't multiply on each load
    const otherUserGuid = 'other-user-demo';

    return [
      {
        tourGuid: 'mock-tour-danube-bike-001',
        userGuid: currentUserGuid,
        name: 'Danube River Bike Tour',
        description: 'Scenic bike ride along the Danube.',
        from: 'Vienna City Center',
        to: 'Greifenstein',
        transportType: Transport.Bike,
        tourDistance: 42.5,
        estimatedTimeMinutes: 180,
        rating: 4,
        tourLogs: [],
      },
      {
        tourGuid: 'mock-tour-alpine-hike-002',
        userGuid: currentUserGuid,
        name: 'Alpine Hike',
        description: 'Challenging hike with mountain views.',
        from: 'Karsee Lake',
        to: 'Zugspitze Peak',
        transportType: Transport.Car,
        tourDistance: 16.2,
        estimatedTimeMinutes: 480,
        rating: 5,
        tourLogs: [],
      },
      {
        tourGuid: 'mock-tour-city-run-003',
        userGuid: otherUserGuid,
        name: 'City Running Route',
        description: 'Fast-paced urban running circuit.',
        from: 'City Center',
        to: 'City Center',
        transportType: Transport.Bike,
        tourDistance: 10.0,
        estimatedTimeMinutes: 45,
        rating: 3,
        tourLogs: [],
      },
      {
        tourGuid: 'mock-tour-weekend-getaway-004',
        userGuid: otherUserGuid,
        name: 'Weekend Getaway',
        description: 'Relaxing vacation route.',
        from: 'Home Town',
        to: 'Beach Resort',
        transportType: Transport.Car,
        tourDistance: 250.0,
        estimatedTimeMinutes: 180,
        rating: 4,
        tourLogs: [],
      },
    ];
  }

  private hydrateFromStorage(): void {
    try {
      const raw = typeof localStorage !== 'undefined' ? localStorage.getItem(TourService.TourStorageKey) : null;
      if (!raw) {
        this._tours.set([]);
        return;
      }

      const parsed = JSON.parse(raw) as Tour[];
      // Basic validation
      if (Array.isArray(parsed)) {
        this._tours.set(parsed);
      } else {
        this._tours.set([]);
      }
    } catch {
      console.error('[TourService] Failed to hydrate tours from storage');
      this._tours.set([]);
    }
  }

  private persistToStorage(): void {
    try {
      if (typeof localStorage !== 'undefined') {
        localStorage.setItem(TourService.TourStorageKey, JSON.stringify(this._tours()));
      }
    } catch (err) {
      console.error('[TourService] Failed to persist tours to storage', err);
    }
  }
}

