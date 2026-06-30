import { Component, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { LucideAngularModule, Bell } from 'lucide-angular';
import { Router } from '@angular/router';
import { Subject, interval, takeUntil, startWith } from 'rxjs';

import { AuthService }                           from '../../../core/services/auth';
import { UserService }                           from '../../../core/services/user';
import { NotificationService, NotificationItem } from '../../../core/services/notification';
import { TimeAgoPipe }                           from '../../pipes/time-ago.pipe';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, DatePipe, TimeAgoPipe, LucideAngularModule],
  templateUrl: './topbar.html',
  styleUrl: './topbar.css'
})
export class TopbarComponent implements OnInit, OnDestroy {

  readonly Bell = Bell;

  // ── User info ─────────────────────────────────────────────────
  userName        = '';
  userEmail       = '';
  userDesignation = '';
  initials        = '';
  role            = '';
  profileImageUrl = '';

  // ── Profile dropdown ──────────────────────────────────────────
  dropdownOpen = false;

  // ── Notifications ─────────────────────────────────────────────
  notifications: NotificationItem[] = [];
  notifOpen     = false;
  unreadCount   = 0;

  private destroy$ = new Subject<void>();

  constructor(
    private authService:         AuthService,
    private userService:         UserService,
    private notificationService: NotificationService,
    private router:              Router,
    private elRef:               ElementRef
  ) {}

  ngOnInit(): void {
    this.userName  = this.authService.getUserName();
    this.userEmail = this.authService.getUserEmail();
    this.initials  = this.authService.getUserInitials();
    this.role      = this.authService.currentRole();

    // Ensure a User row exists for this Keycloak user (important for admin/manager
    // who may not have been added via the normal employee creation flow)
    this.userService.ensureCurrentUser().subscribe({ error: () => {} });

    this.userService.getCurrentUser().subscribe({
      next: u => {
        this.userDesignation = (u.designationName && u.designationName !== 'Unassigned')
          ? u.designationName
          : '';
        this.profileImageUrl = u.profileImageUrl ?? '';
      },
      error: () => {}
    });

    // Poll notifications every 30 s; fires immediately on subscribe
    interval(30_000)
      .pipe(startWith(0), takeUntil(this.destroy$))
      .subscribe(() => this.loadNotifications());
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ── Notifications ─────────────────────────────────────────────
  loadNotifications(): void {
    this.notificationService.getNotifications().subscribe({
      next: items => {
        this.notifications = items;
        this.unreadCount   = items.filter(n => !n.isRead).length;
      },
      error: () => {}
    });
  }

  toggleNotif(): void {
    this.notifOpen = !this.notifOpen;
    if (this.notifOpen) this.loadNotifications();
    this.dropdownOpen = false;
  }

  markAllRead(): void {
    this.notificationService.markAllRead().subscribe({
      next: () => {
        this.notifications = this.notifications.map(n => ({ ...n, isRead: true }));
        this.unreadCount   = 0;
      },
      error: () => {}
    });
  }

  getNotifIcon(type: string): string {
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

  // ── Profile dropdown ──────────────────────────────────────────
  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
    this.notifOpen    = false;
  }

  navigate(path: string): void {
    this.dropdownOpen = false;
    this.router.navigate([path]);
  }

  signOut(): void {
    this.dropdownOpen = false;
    this.authService.logout();
  }

  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
      this.notifOpen    = false;
    }
  }
}
