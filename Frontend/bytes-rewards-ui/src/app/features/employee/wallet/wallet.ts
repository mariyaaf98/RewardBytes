import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

import { WalletService } from '../../../core/services/wallet';
import { UserService } from '../../../core/services/user';
import { RedemptionService } from '../../../core/services/redemption';
import { AuthService } from '../../../core/services/auth';

import { LedgerEntry, Transaction } from '../../../core/models/wallet';
import { RedemptionHistory } from '../../../core/models/redemption';

type FilterTab = 'all' | 'credit' | 'debit';

@Component({
  selector: 'app-wallet',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent, DatePipe, FormsModule],
  templateUrl: './wallet.html',
  styleUrl: './wallet.css'
})
export class WalletComponent implements OnInit {

  private readonly walletService     = inject(WalletService);
  private readonly userService       = inject(UserService);
  private readonly redemptionService = inject(RedemptionService);
  private readonly authService       = inject(AuthService);
  private readonly router            = inject(Router);

  readonly employeeMenu = EMPLOYEE_MENU;

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

  // ── State ──────────────────────────────────────────────────────
  readonly availableBytes   = signal<number | null>(null);
  readonly allTransactions  = signal<Transaction[]>([]);
  readonly isLoadingWallet  = signal(true);
  readonly isLoadingHistory = signal(true);
  readonly walletError      = signal('');
  readonly searchText       = signal('');
  readonly activeTab        = signal<FilterTab>('all');

  // ── Derived ────────────────────────────────────────────────────
  readonly totalCredits = computed(() =>
    this.allTransactions()
      .filter(t => t.type === 'credit')
      .reduce((s, t) => s + t.bytes, 0)
  );

  readonly totalDebits = computed(() =>
    this.allTransactions()
      .filter(t => t.type === 'debit')
      .reduce((s, t) => s + t.bytes, 0)
  );

  readonly creditCount = computed(() =>
    this.allTransactions().filter(t => t.type === 'credit').length
  );

  readonly debitCount = computed(() =>
    this.allTransactions().filter(t => t.type === 'debit').length
  );

  readonly filtered = computed(() => {
    let list = this.allTransactions();

    // tab filter
    const tab = this.activeTab();
    if (tab !== 'all') list = list.filter(t => t.type === tab);

    // search
    const q = this.searchText().toLowerCase().trim();
    if (q) list = list.filter(t =>
      t.title.toLowerCase().includes(q) ||
      t.subtitle.toLowerCase().includes(q) ||
      t.note.toLowerCase().includes(q)
    );

    return list;
  });

  // ── Lifecycle ──────────────────────────────────────────────────
  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe({
      next: (user) => {
        this.loadBalance(user.id);
        this.loadHistory(user.id);
      },
      error: (err) => {
        const msg = err.error?.detail ?? 'Could not load profile.';
        this.walletError.set(msg);
        this.isLoadingWallet.set(false);
        this.isLoadingHistory.set(false);
      }
    });
  }

  // ── Loaders ────────────────────────────────────────────────────
  private loadBalance(userId: string): void {
    this.walletService.getWallet(userId).subscribe({
      next: res => { this.availableBytes.set(res.availableBytes); this.isLoadingWallet.set(false); },
      error: () => this.isLoadingWallet.set(false)
    });
  }

  private loadHistory(userId: string): void {
    let credits: LedgerEntry[] = [];
    let redemptions: RedemptionHistory[] = [];
    let loaded = 0;

    const merge = () => {
      if (++loaded < 2) return;

      // Build credit transactions (rewards + refunds)
      const creditTxns: Transaction[] = credits.map(e => ({
        id:       e.rewardId,
        type:     'credit' as const,
        title:    e.entryType === 'Refund'
                    ? `Refund: ${e.reason.replace('Refund for rejected redemption: ', '')}`
                    : `Recognition from ${e.awardedBy}`,
        subtitle: e.entryType === 'Refund'
                    ? 'Redemption Refund'
                    : (e.rewardCategoryName || 'Reward'),
        note:     e.entryType === 'Refund' ? '' : e.reason,
        bytes:    e.bytes,
        date:     e.awardedAt
      }));

      // Build debit transactions
      const debitTxns: Transaction[] = redemptions.map(r => ({
        id:       r.redemptionId,
        type:     'debit',
        title:    `Redeemed: ${r.productName}`,
        subtitle: r.status,
        note:     '',
        bytes:    r.redeemedBytes,
        date:     r.redeemedAt
      }));

      // Merge and sort newest first
      const merged = [...creditTxns, ...debitTxns]
        .sort((a, b) => new Date(b.date).getTime() - new Date(a.date).getTime());

      // Compute running balance (newest first means we go backwards)
      // Start from current balance and work back
      let balance = this.availableBytes() ?? 0;
      for (const t of merged) {
        t.runningBalance = balance;
        if (t.type === 'credit') balance -= t.bytes;
        else                     balance += t.bytes;
      }

      this.allTransactions.set(merged);
      this.isLoadingHistory.set(false);
    };

    this.walletService.getWalletLedger(userId).subscribe({
      next: d => { credits = d; merge(); },
      error: () => merge()
    });

    this.redemptionService.getRedemptionHistory(userId).subscribe({
      next: d => { redemptions = d; merge(); },
      error: () => merge()
    });
  }

  // ── Helpers ────────────────────────────────────────────────────
  setTab(t: FilterTab): void { this.activeTab.set(t); }
  onSearch(v: string): void  { this.searchText.set(v); }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  goToRewards():    void { this.router.navigate(['/rewards']); }
  goToRecognize():  void { this.router.navigate(['/employee/appreciations/create']); }
}
