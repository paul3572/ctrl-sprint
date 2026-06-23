import { Component, inject } from '@angular/core';
import { AppPaths } from '../../app.paths';
import { Router, RouterLink } from '@angular/router';
import { AppStateService } from '../../services/app-state.service';
import { FormsModule } from '@angular/forms';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './login.html',
  styleUrls: ['../../app.scss', './login.scss'],
})
export class Login {
  protected readonly AppPaths = AppPaths;
  protected readonly router = inject(Router);
  protected readonly auth = inject(AppStateService);
  protected readonly notification = inject(NotificationService);

  protected email = '';
  protected password = '';

  protected async onLoginClicked() {
    const success = this.auth.login(this.email, this.password);
    if (!success) {
      this.notification.error(this.auth.error() ?? 'Login failed.');
      return;
    }

    await this.router.navigate([AppPaths.home]);
  }
}
