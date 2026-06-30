import { Injectable, inject } from '@angular/core';
import {
  HttpInterceptor,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpErrorResponse,
} from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { Router } from '@angular/router';
import { AppStateService } from './app-state.service';
import { AppPaths } from '../app.paths';
import { AuthTokenStore } from './auth-tokenstore.service';

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private router = inject(Router);
  private tokenStore = inject(AuthTokenStore);

  private readonly apiBase = 'http://localhost:8080';

  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    if (!req.url.startsWith('http')) {
      req = req.clone({
        url: `${this.apiBase}${req.url}`,
        withCredentials: true,
      });
    } else {
      req = req.clone({ withCredentials: true });
    }

    const token = this.tokenStore.get();

    if (token) {
      req = req.clone({
        setHeaders: {
          Authorization: `Bearer ${token}`,
        },
      });
    }

    return next.handle(req).pipe(
      catchError((error: HttpErrorResponse) => {
        if (error.status === 401) {
          this.router.navigate([AppPaths.login]);
        }
        return throwError(() => error);
      }),
    );
  }
}
