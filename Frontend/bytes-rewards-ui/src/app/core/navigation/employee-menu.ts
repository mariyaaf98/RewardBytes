import {
  LayoutDashboard,
  BadgePlus,
  Gift,
  Wallet,
  Trophy,
  Bell,
  UserCircle,
  ShoppingBag
} from 'lucide-angular';

export const EMPLOYEE_MENU = [
  {
    label: 'Dashboard',
    icon: LayoutDashboard,
    route: '/employee'
  },
  {
    label: 'Recognize',
    icon: BadgePlus,
    children: [
      {
        label: 'Create Appreciation',
        route: '/employee/appreciations/create'
      },
      {
        label: 'History',
        route: '/employee/appreciations/history'
      }
    ]
  },
  {
    label: 'Rewards',
    icon: Gift,
    children: [
      {
        label: 'Catalog',
        route: '/rewards'
      },
      {
        label: 'My Rewards',
        route: '/employee/my-rewards'
      },
      {
        label: 'My Redemptions',
        route: '/redemptions'
      }
    ]
  },
  {
    label: 'Wallet',
    icon: Wallet,
    route: '/wallet'
  },
  {
    label: 'Leaderboard',
    icon: Trophy,
    route: '/leaderboard'
  },
  {
    label: 'My Profile',
    icon: UserCircle,
    route: '/profile'
  },
  {
    label: 'Notifications',
    icon: Bell,
    route: '/notifications'
  }
];