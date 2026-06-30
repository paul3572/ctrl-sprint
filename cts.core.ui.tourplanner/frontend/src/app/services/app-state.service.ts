import { computed, Injectable, signal } from '@angular/core';
import { User } from '../models/user';
import { firstValueFrom } from 'rxjs';
import { HttpClient, HttpErrorResponse } from '@angular/common/http';
import { LoginResponse } from '../contracts/LoginResponse';

@Injectable({ providedIn: 'root' })
export class AppStateService {
  private static readonly SessionCookieName = 'tour-guide_session';

  private readonly _user = signal<User | null>(null);

  private _sessionReady = signal(false);
  readonly sessionReady = this._sessionReady.asReadonly();

  readonly currentUser = this._user.asReadonly();
  readonly isAuthenticated = computed(() => this._user() !== null);
  readonly isLoading = signal(false);
  readonly error = signal<string | null>(null);

  constructor(private readonly http: HttpClient) {
    this.restoreSession();
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

      return true;
    } catch (error) {
      const err = error as HttpErrorResponse;

      this.error.set(err?.error?.detail ?? err?.error?.title ?? err?.message ?? 'Unknown error');

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

  private clearCookie(): void {
    if (typeof document === 'undefined') {
      return;
    }

    document.cookie = `${AppStateService.SessionCookieName}=; expires=Thu, 01 Jan 1970 00:00:00 GMT; path=/; SameSite=Lax`;
  }

  async restoreSession(): Promise<void> {
    try {
      const response = await firstValueFrom(this.http.get<LoginResponse>('/api/auth/me'));

      const user: User = {
        userGuid: response.userGuid,
        email: response.email,
        createdAt: new Date(response.createdAt),
        accessToken: response.accessToken,
      };

      this._user.set(user);
    } catch {
      this._user.set(null);
    } finally {
      this._sessionReady.set(true);
    }
  }
}
