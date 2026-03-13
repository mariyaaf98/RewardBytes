// app.routes.ts
// Add new routes here every time you create a new page

import { Routes } from '@angular/router';
import { DashboardComponent }  from './admin/dashboard/dashboard';
import { UsersComponent }      from './admin/users/users';
import { LibrariansComponent } from './admin/librarians/librarians';   // ✅ new

export const routes: Routes = [

  // Admin Dashboard
  { path: 'admin/dashboard',  component: DashboardComponent  },

  // Admin Users
  { path: 'admin/users',      component: UsersComponent      },

  // Admin Librarians ✅ new
  { path: 'admin/librarians', component: LibrariansComponent },

  // Default → go to dashboard
  { path: '', redirectTo: 'admin/dashboard', pathMatch: 'full' },

  // Add more later:
  // { path: 'admin/system', component: SystemComponent },
];