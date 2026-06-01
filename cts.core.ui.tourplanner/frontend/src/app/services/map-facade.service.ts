import { Injectable } from '@angular/core';
import * as L from 'leaflet';

@Injectable({ providedIn: 'root' })
export class MapFacadeService {
  private map: L.Map | null = null;

  initMap(container: HTMLElement): void {
    if (this.map) return;

    if (!(container instanceof HTMLElement)) {
      throw new Error('Leaflet container is not a valid HTMLElement.');
    }

    this.map = L.map(container, {
      zoomControl: true,
      attributionControl: true,
    });

    L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
      attribution: '© OpenStreetMap contributors',
      maxZoom: 19,
    }).addTo(this.map);

    this.map.setView([48.2082, 16.3738], 12);
  }
}
