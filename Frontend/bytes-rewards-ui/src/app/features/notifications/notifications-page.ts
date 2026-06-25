import { Component, inject, OnInit, OnDestroy, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Subject, interval } from 'rxjs';
import { startWith, takeUntil } from 'rxjs/operators';
import { TimeAgoPipe } from '../../shared/pipes/time-ago.pipe';

import { SidebarComponent } from '../../shared/components/sidebar/sidebar';
import { TopbarComponent }  from '../../shared/components/topbar/topbar';

import { EMPLOYEE_MENU } from '../../core/navigation/employee-menu';
import { MANAGER_MENU }  from '../../core/navigation/manager-menu';
import { ADMIN_MENU }    from '../../core/navigation/admin-menu';

import { AuthService }                            from '../../core/services/auth';
import { NotificationService, NotificationItem }  from '../../core/services/notification';

type Filter = 'all' | 'unread';

@Component({
  selector: 'app-notifications-page',
  standalone: true,
  imports: [CommonModule, DatePipe, TimeAgoPipe, SidebarComponent, TopbarComponent],
  templateUrl: './notifications-page.html',
  styleUrl: './notifications-page.css'
})
export class NotificationsPageComponent implements OnInit, OnDestroy {

  private readonly authService         = inject(AuthService);
  private readonly notificationService = inject(NotificationService);
  private readonly destroy$            = new Subject<void>();

  get activeMenu() {
    const r = this.authService.currentRole();
    if (r === 'manager') return MANAGER_MENU;
    if (r === 'admin')   return ADMIN_MENU;
    return EMPLOYEE_MENU;
  }

  get workspaceTitle(): string {
    const r = this.authService.currentRole();
    if (r === 'manager') return 'Manager Workspace';
    if (r === 'admin')   return 'Admin Workspace';
    return 'Employee Workspace';
  }

  readonly notifications  = signal<NotificationItem[]>([]);
  readonly isLoading      = signal(true);
  readonly activeFilter   = signal<Filter>('all');

  readonly unreadCount = computed(() =>
    this.notifications().filter(n => !n.isRead).length
  );

  readonly filtered = computed(() => {
    if (this.activeFilter() === 'unread')
      return this.notifications().filter(n => !n.isRead);
    return this.notifications();
  });

  ngOnInit(): void {
    // Poll every 20 s so the page stays live while open
    interval(20_000)
      .pipe(startWith(0), takeUntil(this.destroy$))
      .subscribe(() => this.load());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  load(): void {
    if (!this.notifications().length) this.isLoading.set(true);
    this.notificationService.getNotifications().subscribe({
      next: items => {
        this.notifications.set(items);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  markAllRead(): void {
    this.notificationService.markAllRead().subscribe({
      next: () => {
        this.notifications.set(
          this.notifications().map(n => ({ ...n, isRead: true }))
        );
      },
      error: () => {}
    });
  }

  markOneRead(n: NotificationItem): void {
    if (n.isRead) return;
    this.notificationService.markRead([n.id]).subscribe({
      next: () => {
        this.notifications.update(list =>
          list.map(item => item.id === n.id ? { ...item, isRead: true } : item)
        );
      },
      error: () => {}
    });
  }

  setFilter(f: Filter): void { this.activeFilter.set(f); }

  getIcon(type: string): string {
    switch (type) {
      case 'RewardReceived':       return '🏅';
      case 'RewardSent':           return '✅';
      case 'TeamRecognition':      return '🎉';
      case 'AppreciationReceived': return '✨';
      case 'AppreciationSent':     return '✅';
      case 'TeamAppreciation':     return '✨';
      case 'RedemptionPending':    return '🛒';
      case 'NewRedemptionRequest': return '🔔';
      case 'RedemptionApproved':   return '✅';
      case 'RedemptionRejected':   return '❌';
      case 'RedemptionDelivered':  return '📦';
      default:                     return '🔔';
    }
  }

  getBgColor(type: string): string {
    if (['RewardReceived', 'TeamRecognition'].includes(type))      return 'bg-amber-50';
    if (['AppreciationReceived', 'TeamAppreciation'].includes(type)) return 'bg-violet-50';
    if (type === 'RedemptionApproved' || type === 'RedemptionDelivered') return 'bg-emerald-50';
    if (type === 'RedemptionRejected')                             return 'bg-red-50';
    return 'bg-slate-50';
  }
}
