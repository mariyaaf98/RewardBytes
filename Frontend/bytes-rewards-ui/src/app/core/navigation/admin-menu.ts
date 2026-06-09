import {
  LayoutDashboard,
  Users,
  Gift,
  ChartColumn,
  ShieldCheck,
  Settings
} from 'lucide-angular';

export const ADMIN_MENU = [
  {
    label: 'Admin Overview',
    icon: LayoutDashboard,
    route: '/'
  },
  {
    label: 'Employees',
    icon: Users,
    route: '/admin/employees'
  },
  {
    label: 'Roles',
    icon: ShieldCheck,
    route: '/admin/roles'
  },
  {
    label: 'Rewards Catalogue',
    icon: Gift,
    route: '/rewards'
  },
  {
    label: 'Analytics',
    icon: ChartColumn,
    route: '/analytics'
  },
  {
    label: 'Roles & Access',
    icon: ShieldCheck,
    route: '/roles'
  },
  {
    label: 'Settings',
    icon: Settings,
    route: '/settings'
  }
];