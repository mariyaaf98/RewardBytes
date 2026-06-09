import { inject } from '@angular/core';

import {
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthService } from '../services/auth';

export const adminGuard: CanActivateFn = () => {

  const authService = inject(AuthService);

  const router = inject(Router);

  // CHECK ADMIN ROLE
  if (authService.hasRole('admin')) {

    return true;

  }

  // REDIRECT NON-ADMIN USERS
  return router.createUrlTree(['/employee']);

};