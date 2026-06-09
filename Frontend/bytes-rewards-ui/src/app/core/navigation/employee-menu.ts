import {
  LayoutDashboard,
  BadgePlus,
  Gift,
  Wallet,
  Trophy,
  Bell
} from 'lucide-angular';

export const EMPLOYEE_MENU = [
  {
    label: 'Dashboard',
    icon: LayoutDashboard,
    route: '/'
  },
  // {
  //   label: 'Recognize',
  //   icon: BadgePlus,
  //   route: '/recognize'
  // },
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
    route: '/rewards'
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
    label: 'Notifications',
    icon: Bell,
    route: '/notifications'
  }
];