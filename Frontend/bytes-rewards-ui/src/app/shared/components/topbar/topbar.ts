import { Component, OnInit, OnDestroy, HostListener, ElementRef } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LucideAngularModule, Search, Bell } from 'lucide-angular';
import { Router } from '@angular/router';
import { Subject, interval, debounceTime, distinctUntilChanged,
         takeUntil, startWith } from 'rxjs';

import { AuthService }        from '../../../core/services/auth';
import { UserService }        from '../../../core/services/user';
import { RewardService,
         RewardResponse }     from '../../../core/services/reward';
import { AppreciationService } from '../../../core/services/appreciation';
import { NotificationService,
         NotificationItem }   from '../../../core/services/notification';
import { Appreciation }       from '../../../core/models/appreciation';
import { TimeAgoPipe }        from '../../pipes/time-ago.pipe';

// ── Unified search result shape ──────────────────────────────────
export interface SearchResult {
  type:     'employee' | 'reward' | 'appreciation';
  id:       string;
  title:    string;   // primary line
  subtitle: string;   // secondary line
  route:    string;   // where to navigate on click
  icon:     string;   // emoji icon
}

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, DatePipe, TimeAgoPipe, FormsModule, LucideAngularModule],
  templateUrl: './topbar.html',
  styleUrl: './topbar.css'
})
export class TopbarComponent implements OnInit, OnDestroy {

  readonly Search = Search;
  readonly Bell   = Bell;

  // ── User info ─────────────────────────────────────────────────
  userName        = '';
  userEmail       = '';
  userDesignation = '';
  initials        = '';
  role            = '';
  profileImageUrl = '';

  // ── Profile dropdown ──────────────────────────────────────────
  dropdownOpen = false;

  // ── Search ────────────────────────────────────────────────────
  searchQuery    = '';
  searchResults: SearchResult[] = [];
  searchOpen     = false;
  isSearching    = false;

  // source data (loaded once)
  private employees:     { id: string; fullName: string }[] = [];
  private rewards:       RewardResponse[]   = [];
  private appreciations: Appreciation[]     = [];

  private searchSubject = new Subject<string>();
  private destroy$      = new Subject<void>();

  constructor(
    private authService:          AuthService,
    private userService:          UserService,
    private rewardService:        RewardService,
    private appreciationService:  AppreciationService,
    private notificationService:  NotificationService,
    private router:               Router,
    private elRef:                ElementRef
  ) {}

  // ── Notifications ─────────────────────────────────────────────
  notifications:     NotificationItem[] = [];
  notifOpen         = false;
  unreadCount       = 0;

  loadNotifications(): void {
    this.notificationService.getNotifications().subscribe({
      next: items => {
        this.notifications = items;
        this.unreadCount   = items.filter(n => !n.isRead).length;
      },
      error: () => {}
    });
  }

  private startNotificationPolling(): void {
    // Poll every 30 s; fires immediately on subscribe (startWith)
    interval(30_000)
      .pipe(startWith(0), takeUntil(this.destroy$))
      .subscribe(() => this.loadNotifications());
  }

  toggleNotif(): void {
    this.notifOpen = !this.notifOpen;
    if (this.notifOpen) this.loadNotifications(); // immediate refresh on open
    this.dropdownOpen = false;
    this.searchOpen   = false;
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
      case 'RewardReceived':        return '🏅';
      case 'RewardSent':            return '✅';
      case 'RewardAssigned':        return '🏅';
      case 'TeamRecognition':       return '🎉';
      case 'AppreciationReceived':  return '✨';
      case 'AppreciationSent':      return '✅';
      case 'TeamAppreciation':      return '✨';
      case 'RedemptionPending':     return '🛒';
      case 'NewRedemptionRequest':  return '🔔';
      case 'RedemptionApproved':    return '✅';
      case 'RedemptionRejected':    return '❌';
      case 'RedemptionDelivered':   return '📦';
      default:                      return '🔔';
    }
  }

  ngOnInit(): void {
    this.userName  = this.authService.getUserName();
    this.userEmail = this.authService.getUserEmail();
    this.initials  = this.authService.getUserInitials();
    this.role      = this.authService.currentRole();

    this.userService.getCurrentUser().subscribe({
      next: u => {
        this.userDesignation = u.designationName;
        this.profileImageUrl = u.profileImageUrl ?? '';
      },
      error: () => {}
    });

    // start polling notifications every 30 s
    this.startNotificationPolling();

    // load search data sources in background
    this.userService.getUserLookup().subscribe({
      next: d => { this.employees = d; }, error: () => {}
    });
    this.rewardService.getRewards().subscribe({
      next: d => { this.rewards = d; }, error: () => {}
    });
    this.appreciationService.getAppreciations().subscribe({
      next: d => { this.appreciations = d; }, error: () => {}
    });

    // debounced search — 200 ms after user stops typing
    this.searchSubject.pipe(
      debounceTime(200),
      distinctUntilChanged(),
      takeUntil(this.destroy$)
    ).subscribe(q => this.runSearch(q));
  }

  ngOnDestroy(): void {
    this.destroy$.next();
    this.destroy$.complete();
  }

  // ── Search input handler ──────────────────────────────────────
  onSearchInput(value: string): void {
    this.searchQuery = value;
    if (!value.trim()) {
      this.searchResults = [];
      this.searchOpen    = false;
      return;
    }
    this.isSearching = true;
    this.searchOpen  = true;
    this.searchSubject.next(value.trim().toLowerCase());
  }

  private runSearch(q: string): void {
    const results: SearchResult[] = [];

    // ── Employees ─────────────────────────────────────────────
    this.employees
      .filter(e => e.fullName.toLowerCase().includes(q))
      .slice(0, 4)
      .forEach(e => results.push({
        type:     'employee',
        id:       e.id,
        title:    e.fullName,
        subtitle: 'Employee',
        route:    this.role === 'admin' ? '/admin/employees' : '/leaderboard',
        icon:     '👤'
      }));

    // ── Rewards ───────────────────────────────────────────────
    this.rewards
      .filter(r =>
        r.fromUserName.toLowerCase().includes(q) ||
        r.toUserName.toLowerCase().includes(q)   ||
        r.rewardCategoryName.toLowerCase().includes(q) ||
        r.reason.toLowerCase().includes(q)
      )
      .slice(0, 4)
      .forEach(r => results.push({
        type:     'reward',
        id:       r.id,
        title:    `${r.fromUserName} → ${r.toUserName}`,
        subtitle: `${r.rewardCategoryName} · ${r.bytes} bytes`,
        route:    this.role === 'manager' ? '/manager/recognize' : '/rewards',
        icon:     '🏅'
      }));

    // ── Appreciations ─────────────────────────────────────────
    this.appreciations
      .filter(a =>
        a.fromUserName.toLowerCase().includes(q) ||
        a.toUserName.toLowerCase().includes(q)   ||
        a.message.toLowerCase().includes(q)
      )
      .slice(0, 4)
      .forEach(a => results.push({
        type:     'appreciation',
        id:       a.id,
        title:    `${a.fromUserName} appreciated ${a.toUserName}`,
        subtitle: a.message.length > 60 ? a.message.slice(0, 60) + '…' : a.message,
        route:    '/employee/appreciations/history',
        icon:     '✨'
      }));

    this.searchResults      = results;
    this.employeeResults     = results.filter(r => r.type === 'employee');
    this.rewardResults       = results.filter(r => r.type === 'reward');
    this.appreciationResults = results.filter(r => r.type === 'appreciation');
    this.isSearching         = false;
  }

  selectResult(result: SearchResult): void {
    this.searchQuery        = '';
    this.searchResults      = [];
    this.employeeResults     = [];
    this.rewardResults       = [];
    this.appreciationResults = [];
    this.searchOpen          = false;
    this.router.navigate([result.route]);
  }

  clearSearch(): void {
    this.searchQuery        = '';
    this.searchResults      = [];
    this.employeeResults     = [];
    this.rewardResults       = [];
    this.appreciationResults = [];
    this.searchOpen          = false;
  }

  // ── Group helpers ─────────────────────────────────────────────
  employeeResults:     SearchResult[] = [];
  rewardResults:       SearchResult[] = [];
  appreciationResults: SearchResult[] = [];

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

  // close both dropdowns when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
      this.searchOpen   = false;
      this.notifOpen    = false;
    }
  }

  // close search on Escape
  @HostListener('document:keydown.escape')
  onEscape(): void { this.clearSearch(); }
}
