import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';

import { AuthService } from '../../../core/services/auth';
import { UserService } from '../../../core/services/user';
import { WalletService } from '../../../core/services/wallet';
import { AppreciationService } from '../../../core/services/appreciation';

import { Appreciation } from '../../../core/models/appreciation';
import { LedgerEntry } from '../../../core/models/wallet';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent, DatePipe],
  templateUrl: './employee-dashboard.html',
  styleUrl: './employee-dashboard.css'
})
export class EmployeeDashboardComponent implements OnInit {

  private readonly auth              = inject(AuthService);
  private readonly userService       = inject(UserService);
  private readonly walletService     = inject(WalletService);
  private readonly appreciationService = inject(AppreciationService);
  private readonly router            = inject(Router);

  readonly employeeMenu = EMPLOYEE_MENU;

  // ── User ───────────────────────────────────────────────────────
  readonly userName       = signal('');
  readonly userFirstName  = signal('');
  readonly currentUserId  = signal('');
  readonly userDept       = signal('');   // current user's department

  // ── Wallet ─────────────────────────────────────────────────────
  readonly availableBytes  = signal<number | null>(null);
  readonly ledgerEntries   = signal<LedgerEntry[]>([]);
  readonly isLoadingWallet = signal(true);

  // ── Appreciations ──────────────────────────────────────────────
  readonly appreciations        = signal<Appreciation[]>([]);
  readonly isLoadingAppreciations = signal(true);

  // ── Derived ────────────────────────────────────────────────────
  readonly receivedCount = computed(() =>
    this.appreciations().filter(a =>
      a.toUserId === this.currentUserId()
    ).length
  );

  readonly recentAppreciations = computed(() =>
    [...this.appreciations()]
      .sort((a, b) =>
        new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime()
      )
      .slice(0, 3)
  );

  readonly recentLedger = computed(() =>
    this.ledgerEntries().slice(0, 2)
  );

  readonly totalAppreciationsCount = computed(() =>
    this.appreciations().length
  );

  // ── Lifecycle ──────────────────────────────────────────────────
  ngOnInit(): void {
    const name = this.auth.getUserName();
    this.userName.set(name);
    this.userFirstName.set(name.split(' ')[0]);

    this.loadAppreciations();

    this.userService.getCurrentUser().subscribe({
      next: (user) => {
        this.currentUserId.set(user.id);
        this.userDept.set(user.departmentName ?? '');
        this.loadWallet(user.id);
      },
      error: () => this.isLoadingWallet.set(false)
    });
  }

  // ── Loaders ────────────────────────────────────────────────────
  private loadWallet(userId: string): void {
    this.walletService.getWallet(userId).subscribe({
      next: (res) => {
        this.availableBytes.set(res.availableBytes);
        this.isLoadingWallet.set(false);
      },
      error: () => this.isLoadingWallet.set(false)
    });

    this.walletService.getWalletLedger(userId).subscribe({
      next: (entries) => this.ledgerEntries.set(entries),
      error: () => {}
    });
  }

  private loadAppreciations(): void {
    this.appreciationService.getAppreciations().subscribe({
      next: (data) => {
        this.appreciations.set(data);
        this.isLoadingAppreciations.set(false);
      },
      error: () => this.isLoadingAppreciations.set(false)
    });
  }

  // ── Navigation ─────────────────────────────────────────────────
  goToWallet(): void {
    this.router.navigate(['/wallet']);
  }

  goToRecognize(): void {
    this.router.navigate(['/employee/appreciations/create']);
  }

  goToAppreciations(): void {
    this.router.navigate(['/employee/appreciations/history']);
  }

  // ── Helpers ────────────────────────────────────────────────────
  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  formatBytes(n: number): string {
    return n.toLocaleString();
  }
}
