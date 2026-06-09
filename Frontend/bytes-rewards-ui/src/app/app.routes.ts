// import { Routes } from '@angular/router';

// import { roleRedirectGuard } from './core/guards/role-redirect-guard';

// import { authGuard } from './core/guards/auth-guard';

// import { adminGuard } from './core/guards/admin-guard';

// import { managerGuard } from './core/guards/manager-guard';

// import { EmployeeDashboardComponent } from './features/dashboard/employee-dashboard/employee-dashboard';

// import { AdminDashboardComponent } from './features/dashboard/admin-dashboard/admin-dashboard';

// import { ManagerDashboardComponent } from './features/dashboard/manager-dashboard/manager-dashboard';

// import { EmployeeManagementComponent } from './features/admin/employee-management/employee-management';




// export const routes: Routes = [

//   // ROLE BASED REDIRECT
//   {
//     path: '',
//     canActivate: [roleRedirectGuard],
//     children: []
//   },

//   // EMPLOYEE
//   {
//     path: 'employee',
//     component: EmployeeDashboardComponent,
//     canActivate: [authGuard]
//   },

// //   {
// //   path: 'employee/appreciations/create',
// //   loadComponent: () =>
// //     import('./features/employee/appreciations/create-appreciation/create-appreciation')
// //       .then(m => m.CreateAppreciationComponent)
// // },

// // {
// //   path: 'employee/appreciations/history',
// //   loadComponent: () =>
// //     import('./features/employee/appreciations/appreciation-history/appreciation-history')
// //       .then(m => m.AppreciationHistoryComponent)
// // },

//   // ADMIN
//   {
//     path: 'admin',
//     component: AdminDashboardComponent,
//     canActivate: [adminGuard]
//   },

//   {
//     path: 'admin/employees',
//     component: EmployeeManagementComponent,
//     canActivate: [adminGuard]
//   },


//   // MANAGER
//   {
//     path: 'manager',
//     component: ManagerDashboardComponent,
//     canActivate: [managerGuard]
//   }

// ];




import { Routes } from '@angular/router';

import { roleRedirectGuard } from './core/guards/role-redirect-guard';

import { authGuard } from './core/guards/auth-guard';
import { adminGuard } from './core/guards/admin-guard';
import { managerGuard } from './core/guards/manager-guard';

import { EmployeeDashboardComponent } from './features/dashboard/employee-dashboard/employee-dashboard';
import { AdminDashboardComponent } from './features/dashboard/admin-dashboard/admin-dashboard';
import { ManagerDashboardComponent } from './features/dashboard/manager-dashboard/manager-dashboard';

import { EmployeeManagementComponent } from './features/admin/employee-management/employee-management';

export const routes: Routes = [

  // ROOT REDIRECT
  {
    path: '',
    canActivate: [roleRedirectGuard],
    children: []
  },

  // EMPLOYEE
  {
    path: 'employee',
    component: EmployeeDashboardComponent,
    canActivate: [authGuard]
  },

  {
    path: 'employee/appreciations/create',
    loadComponent: () =>
      import('./features/employee/appreciations/create-appreciation/create-appreciation')
        .then(m => m.CreateAppreciationComponent),
    canActivate: [authGuard]
  },

  {
    path: 'employee/appreciations/history',
    loadComponent: () =>
      import('./features/employee/appreciations/appreciation-history/appreciation-history')
        .then(m => m.AppreciationHistoryComponent),
    canActivate: [authGuard]
  },

  // ADMIN
  {
    path: 'admin',
    component: AdminDashboardComponent,
    canActivate: [adminGuard]
  },

  {
    path: 'admin/employees',
    component: EmployeeManagementComponent,
    canActivate: [adminGuard]
  },

  // MANAGER
  {
    path: 'manager',
    component: ManagerDashboardComponent,
    canActivate: [managerGuard]
  },

  // FALLBACK
  {
    path: '**',
    redirectTo: ''
  }

];