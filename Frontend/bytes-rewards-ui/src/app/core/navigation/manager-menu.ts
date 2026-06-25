import {
  LayoutDashboard,
  BadgePlus,
  Trophy,
  Gift,
  UserCircle,
  Users,
  Bell
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
    label: 'Team Rewards',
    icon: Users,
    route: '/manager/team-rewards'
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
  },
  {
    label: 'Notifications',
    icon: Bell,
    route: '/notifications'
  }
];
