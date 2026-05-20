import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { AppPaths } from '../../app.paths';
import { NotificationService } from '../../services/notification.service';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [RouterLink],
  templateUrl: './register.html',
  styleUrls: ['../../app.css', './register.css'],
})
export class Register {
  protected readonly AppPaths = AppPaths;
  protected readonly notification = inject(NotificationService);

  protected onRegisterClicked() {
    this.notification.error('Something went wrong');
  }
}
