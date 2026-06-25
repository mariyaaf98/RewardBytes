import {
  LayoutDashboard,
  Users,
  Gift,
  Tag,
  Building2,
  Briefcase,
  ReceiptText,
  Bell
} from 'lucide-angular';

export const ADMIN_MENU = [
  {
    label: 'Overview',
    icon: LayoutDashboard,
    route: '/admin'
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
    label: 'Designations',
    icon: Briefcase,
    route: '/admin/designations'
  },
  {
    label: 'Reward Categories',
    icon: Tag,
    route: '/admin/reward-categories'
  },
  {
    label: 'Reward Items',
    icon: Gift,
    route: '/admin/reward-items'
  },
  {
    label: 'Redemptions',
    icon: ReceiptText,
    route: '/admin/redemptions'
  },
  {
    label: 'Notifications',
    icon: Bell,
    route: '/notifications'
  }
];
