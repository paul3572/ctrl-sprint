import { computed, Injectable, signal } from '@angular/core';
import { Tour } from '../models/tour';
import { TourLog } from '../models/tourLog';
import { Transport } from '../models/transport';
import { AppStateService } from './app-state.service';
import { firstValueFrom } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { TourDto } from '../contracts/TourDto';
import {TourLogDto} from '../contracts/TourLogDto';

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

  constructor(
    private readonly appState: AppStateService,
    private readonly http: HttpClient,
  ) {}

  async addTour(tour: Tour): Promise<boolean> {
    this.isLoading.set(true);
    this.error.set(null);

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
        this.http.post<TourDto>(`/api/tour/${newTour.userGuid}`, {
          userGuid: newTour.userGuid,
          name: newTour.name,
          description: newTour.description,
          from: newTour.from,
          to: newTour.to,
          transportName: newTour.transportType,
          rating: newTour.rating,
        }),
      );

      await this.loadToursFromBackend();

      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
      return false;
    } finally {
      this.isLoading.set(false);
    }
  }

  getTourByGuid(tourGuid: string): Tour | undefined {
    return this._tours().find((t) => t.tourGuid === tourGuid);
  }

  async updateTour(tourGuid: string, updates: Partial<Tour>): Promise<Tour> {
    this.isLoading.set(true);
    this.error.set(null);

    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot update tour: no authenticated user');
    }

    try {
      const response = await firstValueFrom(
        this.http.patch<TourDto>(`/api/tour/${tourGuid}`, {
          userGuid: currentUser.userGuid,
          name: updates.name,
          description: updates.description,
          from: updates.from,
          to: updates.to,
          transportName: updates.transportType,
          rating: updates.rating,
        }),
      );

      await this.loadToursFromBackend();

      return this.mapTourDto(response);
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
      return updates as Tour;
    } finally {
      this.isLoading.set(false);
    }
  }

  async deleteTour(tourGuid: string): Promise<void> {
    this.isLoading.set(true);
    this.error.set(null);

    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot delete tour: no authenticated user');
    }

    try {
      const response = await firstValueFrom(this.http.delete<TourDto>(`/api/tour/${tourGuid}`));

      await this.loadToursFromBackend();
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  async addTourLog(tourGuid: string, tourLog: TourLog): Promise<void> {
    this.isLoading.set(true);
    this.error.set(null);

    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot add tour: no authenticated user');
    }

    try {
      const response = await firstValueFrom(
        this.http.post<TourLog>(`/api/tourlog/${tourGuid}`, {
          tourGuid: tourGuid,
          timestamp: tourLog.timestamp.toISOString(),
          comment: tourLog.comment,
          difficulty: tourLog.difficulty,
          totalDistanceInMeters: tourLog.totalDistanceInMeters,
          totalTimeMin: tourLog.totalTimeMin,
          rating: tourLog.rating,
        }),
      );

      const newTourLog: TourLog = {
        ...response,
      };

      this._tours.update((tours) =>
        tours.map((t) => {
          if (t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid) {
            return {
              ...t,
              tourLogs: [newTourLog, ...(t.tourLogs ?? [])],
            };
          }
          return t;
        }),
      );
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
    } finally {
      this.isLoading.set(false);
    }
  }

  async deleteTourLog(tourGuid: string, tourLogGuid: string): Promise<void> {
    this.isLoading.set(true);
    this.error.set(null);

    const currentUser = this.appState.currentUser();
    if (!currentUser) {
      throw new Error('Cannot delete tour log: no authenticated user');
    }

    try {
      const response = await firstValueFrom(
        this.http.delete<TourLog>(`/api/tourlog/${tourLogGuid}`),
      );

      const deletedTourLog: TourLog = {
        ...response,
      };

      this._tours.update((tours) =>
        tours.map((t) => {
          if (t.tourGuid === tourGuid && t.userGuid === currentUser.userGuid) {
            return {
              ...t,
              tourLogs: t.tourLogs?.filter((log) => log.tourLogGuid !== deletedTourLog.tourLogGuid) ?? [],
            };
          }
          return t;
        }),
      );
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
    } finally {
      this.isLoading.set(false);
    }
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
            tourLogs: (t.tourLogs ?? []).map((log) =>
              log.tourLogGuid === updatedLog.tourLogGuid ? { ...log, ...updatedLog } : log,
            ),
          };
        }
        return t;
      }),
    );
    this.persistToStorage();
  }

  async loadToursFromBackend(): Promise<boolean> {
    this.isLoading.set(true);
    this.error.set(null);

    try {
      this.isLoading.set(true);
      this.error.set(null);

      const currentUser = this.appState.currentUser();

      if (!currentUser) {
        throw new Error('Cannot load tours: no authenticated user');
      }

      const response = await firstValueFrom(
        this.http.get<TourDto[]>(`/api/tour?userGuid=${currentUser.userGuid}`),
      );

      const tours = response.map((dto) => this.mapTourDto(dto));

      this._tours.set(tours);

      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }
      return false;
    } finally {
      this.isLoading.set(false);
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

  private mapTourDto(dto: TourDto): Tour {
    return {
      tourGuid: dto.tourGuid,
      userGuid: dto.userGuid,
      name: dto.name,
      description: dto.description,
      from: dto.from,
      to: dto.to,
      transportType: dto.transportName as Transport,
      tourDistance: dto.tourDistanceInMeters,
      estimatedTimeMinutes: dto.estimatedTimeMinutes,
      rating: dto.rating,
      tourLogs: (dto.tourLogs ?? []).map(
        (log): TourLog => ({
          tourLogGuid: log.tourLogGuid,
          tourGuid: log.tourGuid,
          timestamp: new Date(log.timestamp),
          comment: log.comment,
          difficulty: log.difficulty,
          totalDistanceInMeters: log.totalDistanceInMeters,
          totalTimeMin: log.totalTimeMin,
          rating: log.rating,
        }),
      ),
    };
  }
}

