import { computed, Injectable, signal } from '@angular/core';
import { User } from '../models/user';

@Injectable({ providedIn: 'root' })
export class AppStateService {
  private readonly _user = signal<User | null>(null);

  readonly currentUser = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);
}
