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

  {
    path: 'admin/departments',
    loadComponent: () =>
      import('./features/admin/department-management/department-management')
        .then(m => m.DepartmentManagementComponent),
    canActivate: [adminGuard]
  },

  {
    path: 'admin/reward-categories',
    loadComponent: () =>
      import('./features/admin/reward-category-management/reward-category-management')
        .then(m => m.RewardCategoryManagementComponent),
    canActivate: [adminGuard]
  },

  // MANAGER
  {
    path: 'manager',
    component: ManagerDashboardComponent,
    canActivate: [managerGuard]
  },

  {
    path: 'manager/recognize',
    loadComponent: () =>
      import('./features/manager/assign-reward/assign-reward')
        .then(m => m.AssignRewardComponent),
    canActivate: [managerGuard]
  },

  // EMPLOYEE — WALLET
  {
    path: 'wallet',
    loadComponent: () =>
      import('./features/employee/wallet/wallet')
        .then(m => m.WalletComponent),
    canActivate: [authGuard]
  },

  // EMPLOYEE — LEADERBOARD
  {
    path: 'leaderboard',
    loadComponent: () =>
      import('./features/employee/leaderboard/leaderboard')
        .then(m => m.LeaderboardComponent),
    canActivate: [authGuard]
  },

  // EMPLOYEE — PROFILE
  {
    path: 'profile',
    loadComponent: () =>
      import('./features/employee/profile/profile')
        .then(m => m.ProfileComponent),
    canActivate: [authGuard]
  },

  // FALLBACK
  {
    path: '**',
    redirectTo: ''
  }

];