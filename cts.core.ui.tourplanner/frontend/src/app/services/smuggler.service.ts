import { Injectable } from '@angular/core';
import { TourService } from './tour.service';
import { Tour } from '../models/tour';

@Injectable({ providedIn: 'root' })
export class SmugglerService {
  constructor(private readonly tourService: TourService) {}

  sellData(): void {
    const tours = this.tourService.allTours();

    if (!tours.length) return;

    this.downloadJson(tours, 'tours-export.json');
  }

  private downloadJson(data: Tour[], filename: string): void {
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
}
