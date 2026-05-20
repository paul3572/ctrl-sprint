import { Component, Input, Output, EventEmitter } from '@angular/core';
import { CommonModule } from '@angular/common';
import { Notification } from '../../services/notification.service';

@Component({
  selector: 'app-notification',
  standalone: true,
  imports: [CommonModule],
  templateUrl: './notification.component.html',
  styleUrls: ['./notification.component.scss'],
})
export class NotificationComponent {
  @Input() notification!: Notification;
  @Output() close = new EventEmitter<void>();

  onClose() {
    this.close.emit();
  }

  getIcon(): string {
    switch (this.notification.type) {
      case 'error':
        return '✕';
      case 'success':
        return '✓';
      case 'warning':
        return '⚠';
      case 'info':
        return 'ⓘ';
      default:
        return '•';
    }
  }
}
