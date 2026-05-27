import { Component, inject } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { AppPaths } from '../../app.paths';
import { NotificationService } from '../../services/notification.service';
import { AppStateService } from '../../services/app-state.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink, FormsModule],
  templateUrl: './register.html',
  styleUrls: ['../../app.scss', './register.scss'],
})
export class Register {
  protected readonly AppPaths = AppPaths;
  protected readonly router = inject(Router);
  protected readonly auth = inject(AppStateService);
  protected readonly notification = inject(NotificationService);

  protected email = '';
  protected password = '';

  protected async onRegisterClicked() {
    const success = this.auth.register(this.email, this.password);
    if (!success) {
      this.notification.error(this.auth.error() ?? 'Registration failed.');
      return;
    }

    this.notification.success('Account created. You are now signed in.');
    await this.router.navigate([AppPaths.home]);
  }
}
