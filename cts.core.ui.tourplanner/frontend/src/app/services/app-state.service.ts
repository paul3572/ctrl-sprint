import {computed, Injectable, signal} from '@angular/core';
import {User} from '../models/user';

@Injectable({ providedIn: 'root' })
export class AppStateService {
  private static readonly SessionCookieName = 'tp_mock_session';

  private readonly _user = signal<User | null>(null);

  readonly currentUser = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);

  constructor() {
    this.hydrateFromCookie();
  }

  login(email: string, password: string): boolean {
    const normalizedEmail = email.trim().toLowerCase();
    if (!normalizedEmail || !password.trim()) {
      this.error.set('Please provide email and password.');
      return false;
    }

    if (!this.isValidEmail(normalizedEmail)) {
      this.error.set('Not a valid email address.');
      return false;
    }

    const user: User = {
      userGuid: this.generateGuid(),
      email: normalizedEmail,
      createdAt: new Date(),
    };

    this._user.set(user);
    this.error.set(null);
    this.persistToCookie(user);
    return true;
  }

  register(email: string, password: string): boolean {
    return this.login(email, password);
  }

  logout(): void {
    this._user.set(null);
    this.error.set(null);
    this.clearCookie();
  }

  private hydrateFromCookie(): void {
    const raw = this.readCookie(AppStateService.SessionCookieName);
    if (!raw) {
      this._user.set(null);
      return;
    }

    try {
      const parsed = JSON.parse(raw) as { guid: string; email: string; createdAt: string };
      if (!parsed?.guid || !parsed?.email || !parsed?.createdAt) {
        this.clearCookie();
        return;
      }

      this._user.set({
        userGuid: parsed.guid,
        email: parsed.email,
        createdAt: new Date(parsed.createdAt),
      });
    } catch {
      this.clearCookie();
      this._user.set(null);
    }
  }

  private persistToCookie(user: User): void {
    if (typeof document === 'undefined') {
      return;
    }

    const expires = new Date();
    expires.setDate(expires.getDate() + 7);

    const payload = encodeURIComponent(
      JSON.stringify({
        guid: user.userGuid,
        email: user.email,
        createdAt: user.createdAt.toISOString(),
      }),
    );

    document.cookie = `${AppStateService.SessionCookieName}=${payload}; expires=${expires.toUTCString()}; path=/; SameSite=Lax`;
  }

  private readCookie(name: string): string | null {
    if (typeof document === 'undefined') {
      return null;
    }

    const prefix = `${name}=`;
    const cookie = document.cookie
      .split(';')
      .map((part) => part.trim())
      .find((part) => part.startsWith(prefix));

    if (!cookie) {
      return null;
    }

    return decodeURIComponent(cookie.substring(prefix.length));
  }

  private clearCookie(): void {
    if (typeof document === 'undefined') {
      return;
    }

    document.cookie = `${AppStateService.SessionCookieName}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax`;
  }

  private isValidEmail(email: string): boolean {
    return /^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(email.trim());
  }

  private generateGuid(): string {
    if (typeof crypto !== 'undefined' && typeof crypto.randomUUID === 'function') {
      return crypto.randomUUID();
    }

    return `user-${Date.now()}`;
  }
}
