import { Injectable, signal } from '@angular/core';

export interface Notification {
  id: string;
  message: string;
  type: 'error' | 'success' | 'warning' | 'info';
  autoClose?: boolean;
  duration?: number; // ms
}

@Injectable({ providedIn: 'root' })
export class NotificationService {
  readonly notifications = signal<Notification[]>([]);

  error(message: string, duration = 5000) {
    this.add(message, 'error', duration);
  }

  success(message: string, duration = 3000) {
    this.add(message, 'success', duration);
  }

  warning(message: string, duration = 4000) {
    this.add(message, 'warning', duration);
  }

  info(message: string, duration = 3000) {
    this.add(message, 'info', duration);
  }

  private add(message: string, type: Notification['type'], duration: number) {
    const id = `notif-${Date.now()}-${Math.random()}`;
    const notification: Notification = {
      id,
      message,
      type,
      autoClose: true,
      duration,
    };

    this.notifications.update((n) => [...n, notification]);

    // Auto-remove after duration
    if (duration > 0) {
      setTimeout(() => {
        this.remove(id);
      }, duration);
    }
  }

  remove(id: string) {
    this.notifications.update((n) => n.filter((notif) => notif.id !== id));
  }
}
