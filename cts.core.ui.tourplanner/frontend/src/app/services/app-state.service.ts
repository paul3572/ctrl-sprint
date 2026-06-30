import { computed, Injectable, signal } from '@angular/core';
import { User } from '../models/user';
import { firstValueFrom } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { LoginResponse } from '../contracts/LoginResponse';

@Injectable({ providedIn: 'root' })
export class AppStateService {
  private static readonly SessionCookieName = 'tour-guide_session';

  private readonly _user = signal<User | null>(null);

  readonly currentUser = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);

  constructor(private readonly http: HttpClient) {
    this.hydrateFromCookie();
  }

  async login(email: string, password: string): Promise<boolean> {
    this.isLoading.set(true);
    this.error.set(null);

    try {
      const response = await firstValueFrom(
        this.http.post<LoginResponse>('/api/auth/login', {
          email,
          password,
        }),
      );

      const user: User = {
        userGuid: response.userGuid,
        email: response.email,
        createdAt: new Date(response.createdAt),
        accessToken: response.accessToken,
      };

      this._user.set(user);
      this.persistToCookie(user);

      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        this.error.set(error.error?.detail ?? error.message);
      } else {
        this.error.set('An unknown error occurred.');
      }

      return false;
    } finally {
      this.isLoading.set(false);
    }
  }

  async register(email: string, password: string): Promise<boolean> {
    this.isLoading.set(true);
    this.error.set(null);

    try {
      const response = await firstValueFrom(
        this.http.post<LoginResponse>('/api/auth/register', {
          email,
          password,
        }),
      );

      const user: User = {
        userGuid: response.userGuid,
        email: response.email,
        createdAt: new Date(response.createdAt),
        accessToken: response.accessToken,
      };

      this._user.set(user);
      this.persistToCookie(user);

      return true;
    } catch (error) {
      if (error instanceof HttpErrorResponse) {
        const backend = error.error;

        this.error.set(
          backend?.detail ?? backend?.errors?.Password?.[0] ?? backend?.title ?? error.message,
        );
      } else {
        this.error.set('An unknown error occurred.');
      }

      return false;
    } finally {
      this.isLoading.set(false);
    }
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
      const parsed = JSON.parse(raw) as { guid: string; email: string; createdAt: string; accessToken: string };
      if (!parsed?.guid || !parsed?.email || !parsed?.createdAt) {
        this.clearCookie();
        return;
      }

      this._user.set({
        userGuid: parsed.guid,
        email: parsed.email,
        createdAt: new Date(parsed.createdAt),
        accessToken: parsed.accessToken,
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
        accessToken: user.accessToken,
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
}
