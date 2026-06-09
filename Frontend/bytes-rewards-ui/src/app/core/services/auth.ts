// import { Injectable, inject } from '@angular/core';

// import Keycloak from 'keycloak-js';

// @Injectable({
//   providedIn: 'root'
// })
// export class AuthService {

//   private keycloak = inject(Keycloak);

//   get roles(): string[] {

//     return this.keycloak.realmAccess?.roles || [];
//   }

//   isLoggedIn(): boolean {

//     return this.keycloak.authenticated ?? false;
//   }

//   hasRole(role: string): boolean {

//     return this.roles.includes(role);
//   }

//   currentRole(): string {

//     if (this.hasRole('admin')) {
//       return 'admin';
//     }

//     if (this.hasRole('manager')) {
//       return 'manager';
//     }

//     return 'employee';
//   }

//   isAdmin(): boolean {

//     return this.hasRole('admin');
//   }

//   isManager(): boolean {

//     return this.hasRole('manager');
//   }

//   isEmployee(): boolean {

//     return this.hasRole('employee');
//   }

//   // LOGIN
//   async login(): Promise<void> {

//     await this.keycloak.login({
//       redirectUri: 'http://localhost:4200'
//     });

//   }

//   // LOGOUT
//   logout(): void {

//     this.keycloak.logout({
//       redirectUri: 'http://localhost:4200'
//     });

//   }


// }


import { Injectable, inject } from '@angular/core';

import Keycloak from 'keycloak-js';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private keycloak = inject(Keycloak);


  get roles(): string[] {

    console.log(
      'Authenticated:',
      this.keycloak.authenticated
    );

    console.log(
      'Realm Roles:',
      this.keycloak.realmAccess?.roles
    );

    console.log(
      'Token:',
      this.keycloak.tokenParsed
    );

    return this.keycloak.realmAccess?.roles || [];

  }


  isLoggedIn(): boolean {

    return this.keycloak.authenticated ?? false;

  }


  hasRole(role: string): boolean {

    return this.roles.includes(role);

  }


  currentRole(): string {

    if (this.hasRole('admin')) {

      return 'admin';

    }


    if (this.hasRole('manager')) {

      return 'manager';

    }


    return 'employee';

  }


  isAdmin(): boolean {

    return this.hasRole('admin');

  }


  isManager(): boolean {

    return this.hasRole('manager');

  }


  isEmployee(): boolean {

    return this.hasRole('employee');

  }


  // CURRENT USER NAME
  getUserName(): string {

    return this.keycloak.tokenParsed?.['name'] ?? '';

  }


  // CURRENT USER EMAIL
  getUserEmail(): string {

    return this.keycloak.tokenParsed?.['email'] ?? '';

  }


  // CURRENT USER INITIALS
  getUserInitials(): string {

    const name =
      this.getUserName();


    if (!name) {

      return '';

    }


    return name
      .split(' ')
      .map(value => value[0])
      .join('')
      .toUpperCase();

  }


  // LOGIN
  async login(): Promise<void> {

    await this.keycloak.login({
      redirectUri: 'http://localhost:4200'
    });

  }


  // LOGOUT
  logout(): void {

    this.keycloak.logout({
      redirectUri: 'http://localhost:4200'
    });

  }

}