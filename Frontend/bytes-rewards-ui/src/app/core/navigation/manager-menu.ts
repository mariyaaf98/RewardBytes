import {
  LayoutDashboard,
  ChartColumn,
  ClipboardCheck,
  BadgePlus,
  Gift,
  Bell
} from 'lucide-angular';

export const MANAGER_MENU = [
  {
    label: 'Team Overview',
    icon: LayoutDashboard,
    route: '/manager'
  },
  {
    label: 'Team Insights',
    icon: ChartColumn,
    route: '/manager/insights'
  },
  {
    label: 'Approvals',
    icon: ClipboardCheck,
    route: '/manager/approvals'
  },
  {
    label: 'Recognize',
    icon: BadgePlus,
    route: '/manager/recognize'
  },
  {
    label: 'Rewards',
    icon: Gift,
    route: '/manager/rewards'
  },
  {
    label: 'Notifications',
    icon: Bell,
    route: '/manager/notifications'
  }
];