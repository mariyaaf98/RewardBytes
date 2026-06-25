
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
    path: 'admin/designations',
    loadComponent: () =>
      import('./features/admin/designation-management/designation-management')
        .then(m => m.DesignationManagementComponent),
    canActivate: [adminGuard]
  },

  {
    path: 'admin/reward-categories',
    loadComponent: () =>
      import('./features/admin/reward-category-management/reward-category-management')
        .then(m => m.RewardCategoryManagementComponent),
    canActivate: [adminGuard]
  },

  {
    path: 'admin/reward-items',
    loadComponent: () =>
      import('./features/admin/reward-item-management/reward-item-management')
        .then(m => m.RewardItemManagementComponent),
    canActivate: [adminGuard]
  },

  {
    path: 'admin/redemptions',
    loadComponent: () =>
      import('./features/admin/redemption-management/redemption-management')
        .then(m => m.RedemptionManagementComponent),
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

  {
    path: 'manager/team-rewards',
    loadComponent: () =>
      import('./features/manager/team-rewards/team-rewards')
        .then(m => m.TeamRewardsComponent),
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

  // EMPLOYEE — REWARDS CATALOG
  {
    path: 'rewards',
    loadComponent: () =>
      import('./features/employee/rewards/rewards')
        .then(m => m.RewardsComponent),
    canActivate: [authGuard]
  },

  // EMPLOYEE — MY REWARDS HISTORY
  {
    path: 'employee/my-rewards',
    loadComponent: () =>
      import('./features/employee/my-rewards/my-rewards')
        .then(m => m.MyRewardsComponent),
    canActivate: [authGuard]
  },

  // EMPLOYEE — REDEMPTIONS
  {
    path: 'redemptions',
    loadComponent: () =>
      import('./features/employee/redemptions/redemptions')
        .then(m => m.RedemptionsComponent),
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

  // NOTIFICATIONS — all roles share one page
  {
    path: 'notifications',
    loadComponent: () =>
      import('./features/notifications/notifications-page')
        .then(m => m.NotificationsPageComponent),
    canActivate: [authGuard]
  },

  // FALLBACK
  {
    path: '**',
    redirectTo: ''
  }

];