import { inject } from '@angular/core';

import {
  CanActivateFn,
  Router
} from '@angular/router';

import { AuthService } from '../services/auth';

export const managerGuard: CanActivateFn = () => {

  const authService = inject(AuthService);

  const router = inject(Router);

  const role = authService.currentRole();

  // MANAGER OR ADMIN ACCESS
  if (
    role === 'manager' ||
    role === 'admin'
  ) {

    return true;

  }

  // REDIRECT EMPLOYEE USERS
  return router.createUrlTree(['/employee']);

};