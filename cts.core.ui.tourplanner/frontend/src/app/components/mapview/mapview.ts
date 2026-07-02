import {
  AfterViewInit,
  Component,
  effect,
  ElementRef,
  inject,
  input,
  viewChild,
} from '@angular/core';
import { MapFacadeService } from '../../services/map-facade.service';
import { RouteGeometry } from '../../contracts/RouteGeometry';

@Component({
  selector: 'app-mapview',
  standalone: true,
  templateUrl: './mapview.html',
  styleUrl: './mapview.css',
})
export class Mapview implements AfterViewInit {
  private readonly mapFacade = inject(MapFacadeService);
  readonly routeGeometry = input<RouteGeometry | undefined>(undefined);

  protected readonly mapHost = viewChild.required<ElementRef<HTMLDivElement>>('mapHost');

  private mapInitialized = false;

  private readonly routeEffect = effect(() => {
    const route = this.routeGeometry();

    if (!route) return;
    if (!this.mapInitialized) return;

    if (route && this.mapInitialized) {
      this.mapFacade.setRoute(route);
    }
  });

  ngAfterViewInit(): void {
    const el = this.mapHost().nativeElement;

    if (!el) return;

    this.mapFacade.initMap(this.mapHost().nativeElement);
    this.mapInitialized = true;

    const route = this.routeGeometry();
    if (route) {
      this.mapFacade.setRoute(route);
    }
  }
}
