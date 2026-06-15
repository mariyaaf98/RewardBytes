import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';
import { WalletService } from '../../../core/services/wallet';
import { UserService } from '../../../core/services/user';
import { LedgerEntry } from '../../../core/models/wallet';

@Component({
  selector: 'app-wallet',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent, DatePipe, FormsModule],
  templateUrl: './wallet.html',
  styleUrl: './wallet.css'
})
export class WalletComponent implements OnInit {

  private readonly walletService = inject(WalletService);
  private readonly userService   = inject(UserService);
  private readonly router        = inject(Router);

  readonly employeeMenu = EMPLOYEE_MENU;

  // ── State ──────────────────────────────────────────────────────
  readonly availableBytes  = signal<number | null>(null);
  readonly ledger          = signal<LedgerEntry[]>([]);
  readonly isLoadingWallet = signal(true);
  readonly isLoadingLedger = signal(true);
  readonly walletError     = signal('');
  readonly ledgerError     = signal('');
  readonly searchText      = signal('');

  // ── Derived ────────────────────────────────────────────────────
  readonly totalEarned = computed(() =>
    this.ledger().reduce((sum, e) => sum + e.bytes, 0)
  );

  readonly filteredLedger = computed(() => {
    const q = this.searchText().toLowerCase().trim();
    if (!q) return this.ledger();
    return this.ledger().filter(e =>
      e.rewardCategoryName.toLowerCase().includes(q) ||
      e.awardedBy.toLowerCase().includes(q) ||
      e.reason.toLowerCase().includes(q)
    );
  });

  // ── Lifecycle ──────────────────────────────────────────────────
  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe({
      next: (user) => {
        this.loadWallet(user.id);
        this.loadLedger(user.id);
      },
      error: (err) => {
        const msg = err.error?.detail ?? err.error?.message ?? 'Could not load user profile.';
        this.walletError.set(msg);
        this.ledgerError.set(msg);
        this.isLoadingWallet.set(false);
        this.isLoadingLedger.set(false);
      }
    });
  }

  // ── Data loaders ───────────────────────────────────────────────
  private loadWallet(userId: string): void {
    this.isLoadingWallet.set(true);
    this.walletService.getWallet(userId).subscribe({
      next:  (res) => { this.availableBytes.set(res.availableBytes); this.isLoadingWallet.set(false); },
      error: (err) => { this.walletError.set(err.error?.detail ?? 'Could not load wallet balance.'); this.isLoadingWallet.set(false); }
    });
  }

  private loadLedger(userId: string): void {
    this.isLoadingLedger.set(true);
    this.walletService.getWalletLedger(userId).subscribe({
      next:  (res) => { this.ledger.set(res); this.isLoadingLedger.set(false); },
      error: (err) => { this.ledgerError.set(err.error?.detail ?? 'Could not load transaction history.'); this.isLoadingLedger.set(false); }
    });
  }

  // ── Helpers ────────────────────────────────────────────────────
  onSearch(value: string): void {
    this.searchText.set(value);
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  goToRecognize(): void {
    this.router.navigate(['/employee/appreciations/create']);
  }
}
