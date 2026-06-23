import {
  LayoutDashboard,
  Users,
  Gift,
  Tag,
  ShieldCheck,
  Building2,
  ShoppingBag,
  ReceiptText
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
  }
];
