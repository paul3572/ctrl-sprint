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

@Injectable()
export class AuthInterceptor implements HttpInterceptor {
  private router = inject(Router);
  private auth = inject(AppStateService);

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
