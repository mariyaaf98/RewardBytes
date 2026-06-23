import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';

import { RedemptionService } from '../../../core/services/redemption';
import { UserService } from '../../../core/services/user';

import { RedemptionHistory } from '../../../core/models/redemption';

@Component({
  selector: 'app-redemptions',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, SidebarComponent, TopbarComponent],
  templateUrl: './redemptions.html',
  styleUrl: './redemptions.css'
})
export class RedemptionsComponent implements OnInit {

  private readonly redemptionService = inject(RedemptionService);
  private readonly userService       = inject(UserService);
  private readonly router            = inject(Router);

  readonly employeeMenu = EMPLOYEE_MENU;

  // ── State ───────────────────────────────────────────────────────
  readonly history    = signal<RedemptionHistory[]>([]);
  readonly isLoading  = signal(true);
  readonly error      = signal('');
  readonly searchText = signal('');
  readonly statusFilter = signal<string>('all');

  // ── Derived ─────────────────────────────────────────────────────
  readonly filtered = computed(() => {
    let list = this.history();

    const q = this.searchText().toLowerCase().trim();
    if (q) list = list.filter(r => r.productName.toLowerCase().includes(q));

    const s = this.statusFilter();
    if (s !== 'all') list = list.filter(r => r.status === s);

    return list;
  });

  readonly totalCount     = computed(() => this.history().length);
  readonly pendingCount   = computed(() => this.history().filter(r => r.status === 'Pending').length);
  readonly approvedCount  = computed(() => this.history().filter(r => r.status === 'Approved' || r.status === 'Delivered').length);
  readonly totalRedeemed  = computed(() => this.history().reduce((s, r) => s + r.redeemedBytes, 0));

  // ── Lifecycle ───────────────────────────────────────────────────
  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe({
      next: (user) => this.loadHistory(user.id),
      error: (err) => {
        this.error.set(err.error?.detail ?? 'Could not load user.');
        this.isLoading.set(false);
      }
    });
  }

  loadHistory(userId: string): void {
    this.isLoading.set(true);
    this.redemptionService.getRedemptionHistory(userId).subscribe({
      next: (data) => { this.history.set(data); this.isLoading.set(false); },
      error: (err) => { this.error.set(err.error?.detail ?? 'Failed to load redemptions.'); this.isLoading.set(false); }
    });
  }

  onSearch(v: string): void { this.searchText.set(v); }
  setStatus(s: string): void { this.statusFilter.set(s); }

  goToRewards(): void { this.router.navigate(['/rewards']); }

  // Status badge styling
  statusClass(status: string): string {
    switch (status) {
      case 'Pending':   return 'bg-amber-50 text-amber-600 border border-amber-200';
      case 'Approved':  return 'bg-blue-50 text-blue-600 border border-blue-200';
      case 'Delivered': return 'bg-emerald-50 text-emerald-600 border border-emerald-200';
      case 'Rejected':  return 'bg-red-50 text-red-500 border border-red-200';
      default:          return 'bg-slate-100 text-slate-500';
    }
  }

  statusIcon(status: string): string {
    switch (status) {
      case 'Pending':   return '⏳';
      case 'Approved':  return '✓';
      case 'Delivered': return '📦';
      case 'Rejected':  return '✕';
      default:          return '•';
    }
  }
}
