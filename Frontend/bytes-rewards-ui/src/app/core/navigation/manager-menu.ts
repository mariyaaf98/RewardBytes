import {
  LayoutDashboard,
  BadgePlus,
  Trophy,
  Gift,
  UserCircle
} from 'lucide-angular';

export const MANAGER_MENU = [
  {
    label: 'Team Overview',
    icon: LayoutDashboard,
    route: '/manager'
  },
  {
    label: 'Recognize',
    icon: BadgePlus,
    route: '/manager/recognize'
  },
  {
    label: 'Leaderboard',
    icon: Trophy,
    route: '/leaderboard'
  },
  {
    label: 'Rewards Catalog',
    icon: Gift,
    route: '/rewards'
  },
  {
    label: 'My Profile',
    icon: UserCircle,
    route: '/profile'
  }
];
