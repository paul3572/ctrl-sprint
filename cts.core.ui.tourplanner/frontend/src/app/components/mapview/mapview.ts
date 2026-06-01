import { AfterViewInit, Component, ElementRef, inject, viewChild } from '@angular/core';
import { MapFacadeService } from '../../services/map-facade.service';

@Component({
  selector: 'app-mapview',
  standalone: true,
  templateUrl: './mapview.html',
  styleUrl: './mapview.css',
})
export class Mapview implements AfterViewInit {
  private readonly mapFacade = inject(MapFacadeService);

  protected readonly mapHost = viewChild.required<ElementRef<HTMLDivElement>>('mapHost');

  ngAfterViewInit(): void {
    this.mapFacade.initMap(this.mapHost().nativeElement);
  }
}
