import {
  LayoutDashboard,
  Users,
  Gift,
  ChartColumn,
  ShieldCheck,
  Settings,
  Building2,
  Tag
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
    label: 'Departments',
    icon: Building2,
    route: '/admin/departments'
  },
  {
    label: 'Reward Categories',
    icon: Tag,
    route: '/admin/reward-categories'
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
    label: 'Settings',
    icon: Settings,
    route: '/settings'
  }
];