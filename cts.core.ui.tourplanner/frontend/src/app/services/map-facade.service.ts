import { inject, Injectable } from '@angular/core';
import * as L from 'leaflet';
import { RouteGeometry } from '../contracts/RouteGeometry';
import { NotificationService } from './notification.service';

@Injectable({ providedIn: 'root' })
export class MapFacadeService {
  private map: L.Map | null = null;
  private routeLine: L.Polyline | null = null;
  private readonly notifications = inject(NotificationService);

  initMap(container: HTMLElement): void {
    if (this.map) {
      this.map.remove();
      this.map = null;
    }

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

  setRoute(route: RouteGeometry): void {
    if (!this.map) return;

    if (!route.coordinates || !Array.isArray(route.coordinates) || route.coordinates.length === 0) {
      this.notifications.warning('No valid coordinates provided for the route. Unable to display on map.');
      return;
    }

    let latLngs: [number, number][] = [];

    try {
      const firstCoord = route.coordinates[0];

      if (Array.isArray(firstCoord)) {
        // Backend format: [longitude, latitude]
        latLngs = (route.coordinates as any).map(
          (c: number[]) => [c[1], c[0]] as [number, number],
        );
      } else if (
        typeof firstCoord === 'object' &&
        firstCoord !== null &&
        'latitude' in firstCoord
      ) {
        // Frontend RouteCoordinates format
        latLngs = (route.coordinates as any).map(
          (c: any) => [c.latitude, c.longitude] as [number, number],
        );
      } else {
        return;
      }

      if (latLngs.length === 0) {
        return;
      }

      if (this.routeLine) {
        this.map.removeLayer(this.routeLine);
      }

      this.routeLine = L.polyline(latLngs, {
        color: 'blue',
        weight: 4,
      }).addTo(this.map);

      this.map.fitBounds(this.routeLine.getBounds());
    } catch (error) {
      this.notifications.warning('Failed to set route on the map. Please check the route data.');
    }
  }
}
