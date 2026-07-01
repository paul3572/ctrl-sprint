import { Injectable } from '@angular/core';
import { TourService } from './tour.service';
import { Tour } from '../models/tour';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { TourDto } from '../contracts/TourDto';

@Injectable({ providedIn: 'root' })
export class SmugglerService {
  constructor(private readonly tourService: TourService, private readonly http: HttpClient) {}

  exportData(): void {
    const tours = this.tourService.allTours().map(t => this.mapTourToDto(t));

    if (!tours.length) return;

    this.downloadJson(tours, 'tours-export.json');
  }

  private downloadJson(data: TourDto[], filename: string): void {
    const blob = new Blob([JSON.stringify(data, null, 2)], {
      type: 'application/json',
    });

    const url = window.URL.createObjectURL(blob);

    const a = document.createElement('a');
    a.href = url;
    a.download = filename;
    a.click();

    window.URL.revokeObjectURL(url);
  }

  async importData(userGuid: string, file: File): Promise<void> {
    const tours = await this.readFile(file);

    if (!tours.length) return;

    await this.pushToBackend(userGuid, tours);

    await this.tourService.loadToursFromBackend();
  }

  private async readFile(file: File): Promise<Tour[]> {
    return new Promise((resolve, reject) => {
      const reader = new FileReader();

      reader.onload = () => {
        try {
          const json = JSON.parse(reader.result as string);
          resolve(json as Tour[]);
        } catch (err) {
          reject(err);
        }
      };

      reader.onerror = () => reject(reader.error);

      reader.readAsText(file);
    });
  }

  private async pushToBackend(userGuid: string, tours: Tour[]): Promise<void> {
    const dto: TourDto[] = tours.map(t => this.mapTourToDto(t));

    await firstValueFrom(
      this.http.post<Tour[]>(`/api/tour/buyData/${userGuid}`, dto, {
        headers: {
          'Content-Type': 'application/json',
        },
      })
    );
  }

  private mapTourToDto(tour: Tour): TourDto {
    return {
      tourGuid: tour.tourGuid,
      userGuid: tour.userGuid,
      name: tour.name,
      description: tour.description,
      from: tour.from,
      to: tour.to,

      transportName: tour.transportType,

      tourDistanceInMeters: tour.tourDistance,
      estimatedTimeMinutes: tour.estimatedTimeMinutes,
      rating: tour.rating,

      tourLogs: (tour.tourLogs ?? []).map(log => ({
        tourLogGuid: log.tourLogGuid,
        tourGuid: log.tourGuid,
        timestamp: log.timestamp.toString(),
        comment: log.comment,
        difficulty: log.difficulty,
        totalDistanceInMeters: log.totalDistanceInMeters,
        totalTimeMin: log.totalTimeMin,
        rating: log.rating,
      })),

      routeGeometry: tour.routeGeometry,
    };
  }
}
