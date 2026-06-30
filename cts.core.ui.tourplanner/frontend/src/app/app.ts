import { Component, effect, inject, signal } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { NotificationContainerComponent } from './components/notification-container/notification-container.component';
import { AppStateService } from './services/app-state.service';
import { TourService } from './services/tour.service';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, NotificationContainerComponent],
  templateUrl: './app.html',
  styleUrls: ['./app.scss'],
})
export class App {
  protected readonly title = signal('frontend');

  private appState = inject(AppStateService);
  private tourService = inject(TourService);

  constructor() {
    effect(() => {
      if (this.appState.sessionReady() && this.appState.currentUser()) {
        this.tourService.loadToursFromBackend();
      }
    });
  }
}
