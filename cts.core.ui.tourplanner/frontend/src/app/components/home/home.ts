import { Component, inject } from '@angular/core';
import { Router } from '@angular/router';
import { AppPaths } from '../../app.paths';
import { AppStateService } from '../../services/app-state.service';

@Component({
  selector: 'app-home',
  templateUrl: './home.html',
  styleUrls: ['../../app.scss', './home.css'],
})
export class Home {
  private readonly router = inject(Router);
  private readonly appState = inject(AppStateService);

  protected async onLogoutClicked(): Promise<void> {
    this.appState.logout();
    await this.router.navigate([AppPaths.login]);
  }
}
