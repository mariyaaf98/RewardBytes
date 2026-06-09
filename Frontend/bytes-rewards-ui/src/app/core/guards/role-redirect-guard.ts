import { inject } from '@angular/core';

import {
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthService } from '../services/auth';

export const roleRedirectGuard: CanActivateFn = async () => {

  const authService = inject(AuthService);

  const router = inject(Router);

  // NOT LOGGED IN
  if (!authService.isLoggedIn()) {

    await authService.login();

    return false;

  }

  // ADMIN
  if (authService.hasRole('admin')) {

    return router.createUrlTree(['/admin']);

  }

  // MANAGER
  if (authService.hasRole('manager')) {

    return router.createUrlTree(['/manager']);

  }

  // EMPLOYEE
  return router.createUrlTree(['/employee']);

};