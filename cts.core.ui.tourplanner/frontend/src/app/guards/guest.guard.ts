import { CanActivateFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AppStateService } from '../services/app-state.service';
import { AppPaths } from '../app.paths';

export const guestGuard: CanActivateFn = () => {
  const auth = inject(AppStateService);
  const router = inject(Router);

  if (!auth.sessionReady()) {
    return true;
  }

  if (!auth.isAuthenticated()) {
    return true;
  }

  return router.createUrlTree([AppPaths.home]);
};
