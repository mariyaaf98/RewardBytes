import {
  HttpInterceptorFn
} from '@angular/common/http';

import {
  inject
} from '@angular/core';

import Keycloak from 'keycloak-js';

export const authInterceptor: HttpInterceptorFn = (req, next) => {

  const keycloak = inject(Keycloak);

  const token = keycloak.token;

  // IF TOKEN DOES NOT EXIST
  if (!token) {
    return next(req);
  }

  // ATTACH TOKEN
  const clonedRequest = req.clone({
    setHeaders: {
      Authorization: `Bearer ${token}`
    }
  });

  return next(clonedRequest);
};