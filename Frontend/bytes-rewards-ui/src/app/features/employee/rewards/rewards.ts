import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

import { RewardCatalogService } from '../../../core/services/reward-catalog';
import { RedemptionService } from '../../../core/services/redemption';
import { UserService } from '../../../core/services/user';
import { WalletService } from '../../../core/services/wallet';
import { AuthService } from '../../../core/services/auth';

import { RewardItem } from '../../../core/models/reward-item';

@Component({
  selector: 'app-rewards',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, TopbarComponent],
  templateUrl: './rewards.html',
  styleUrl: './rewards.css'
})
export class RewardsComponent implements OnInit {

  private readonly catalogService    = inject(RewardCatalogService);
  private readonly redemptionService = inject(RedemptionService);
  private readonly userService       = inject(UserService);
  private readonly walletService     = inject(WalletService);
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

  // role helpers
  get isManager(): boolean { return this.authService.currentRole() === 'manager'; }
  get isAdmin():   boolean { return this.authService.currentRole() === 'admin'; }

  // ── State ───────────────────────────────────────────────────────
  readonly items          = signal<RewardItem[]>([]);
  readonly isLoading      = signal(true);
  readonly error          = signal('');
  readonly searchText     = signal('');
  readonly availableBytes = signal<number | null>(null);

  // Track which rewardItemIds have an active (Pending/Approved) redemption
  readonly activeRedemptions = signal<Map<string, string>>(new Map());
  // Map: rewardItemId → status ('Pending' | 'Approved')

  // Redeem state
  readonly redeemingId   = signal<string | null>(null);
  readonly redeemSuccess = signal('');
  readonly redeemError   = signal('');

  private currentUserId = '';

  // ── Derived ─────────────────────────────────────────────────────
  readonly filtered = computed(() => {
    const q = this.searchText().toLowerCase().trim();
    if (!q) return this.items();
    return this.items().filter(i =>
      i.name.toLowerCase().includes(q) ||
      i.description.toLowerCase().includes(q) ||
      i.productCode.toLowerCase().includes(q)
    );
  });

  readonly totalItems    = computed(() => this.items().length);
  readonly affordableCount = computed(() =>
    this.availableBytes() !== null
      ? this.items().filter(i => i.requiredBytes <= this.availableBytes()!).length
      : 0
  );
  readonly minBytes = computed(() =>
    this.items().length ? Math.min(...this.items().map(i => i.requiredBytes)) : 0
  );

  // ── Lifecycle ───────────────────────────────────────────────────
  ngOnInit(): void {
    this.loadItems();

    // Only load wallet for employees — managers don't redeem
    if (!this.isManager && !this.isAdmin) {
      this.userService.getCurrentUser().subscribe({
        next: (user) => {
          this.currentUserId = user.id;
          this.walletService.getWallet(user.id).subscribe({
            next: (w) => this.availableBytes.set(w.availableBytes),
            error: () => {}
          });
          // Load existing redemptions to know which items are already pending
          this.loadActiveRedemptions(user.id);
        },
        error: () => {}
      });
    }
  }

  private loadActiveRedemptions(userId: string): void {
    this.redemptionService.getRedemptionHistory(userId).subscribe({
      next: (list) => {
        const map = new Map<string, string>();

        // Group by product name — take the most recent redemption per item
        // Include Pending, Approved (blocked) and Rejected (show notification + allow retry)
        const sorted = [...list].sort(
          (a, b) => new Date(b.redeemedAt).getTime() - new Date(a.redeemedAt).getTime()
        );

        sorted.forEach(r => {
          const item = this.items().find(i => i.name === r.productName);
          if (item && !map.has(item.id)) {
            // Only track the most recent redemption per item
            if (r.status === 'Pending' || r.status === 'Approved' || r.status === 'Rejected') {
              map.set(item.id, r.status);
            }
          }
        });

        this.activeRedemptions.set(map);

        // If any Rejected items found, refresh wallet — bytes were refunded server-side
        const hasRejected = [...map.values()].some(s => s === 'Rejected');
        if (hasRejected) {
          this.walletService.getWallet(userId).subscribe({
            next: (w) => this.availableBytes.set(w.availableBytes),
            error: () => {}
          });
        }
      },
      error: () => {}
    });
  }

  loadItems(): void {
    this.isLoading.set(true);
    this.catalogService.getRewardItems().subscribe({
      next: (data) => { this.items.set(data); this.isLoading.set(false); },
      error: (err) => { this.error.set(err.error?.detail ?? 'Failed to load rewards.'); this.isLoading.set(false); }
    });
  }

  onSearch(v: string): void { this.searchText.set(v); }

  // Get the current redemption status for an item (null = no active redemption)
  getItemRedemptionStatus(itemId: string): string | null {
    return this.activeRedemptions().get(itemId) ?? null;
  }

  // Names of items whose most recent redemption was rejected — for the banner
  readonly rejectedItemNames = computed(() => {
    const names: string[] = [];
    this.activeRedemptions().forEach((status, itemId) => {
      if (status === 'Rejected') {
        const item = this.items().find(i => i.id === itemId);
        if (item) names.push(item.name);
      }
    });
    return names;
  });

  canAfford(item: RewardItem): boolean {
    return this.availableBytes() !== null && this.availableBytes()! >= item.requiredBytes;
  }

  redeem(item: RewardItem): void {
    if (!this.currentUserId) return;
    this.redeemingId.set(item.id);
    this.redeemError.set('');
    this.redeemSuccess.set('');

    this.redemptionService.redeemReward(this.currentUserId, item.id).subscribe({
      next: () => {
        this.redeemingId.set(null);
        this.redeemSuccess.set(
          `"${item.name}" redemption request submitted! ` +
          `Bytes have been deducted from your wallet. ` +
          `Your request is now pending admin approval.`
        );
        // Refresh wallet balance
        this.walletService.getWallet(this.currentUserId).subscribe({
          next: (w) => this.availableBytes.set(w.availableBytes),
          error: () => {}
        });
        // Mark this item as pending immediately in UI
        const map = new Map(this.activeRedemptions());
        map.set(item.id, 'Pending');
        this.activeRedemptions.set(map);

        setTimeout(() => this.redeemSuccess.set(''), 8000);
      },
      error: (err) => {
        this.redeemingId.set(null);
        this.redeemError.set(err.error?.detail ?? err.error?.message ?? 'Redemption failed.');
        setTimeout(() => this.redeemError.set(''), 5000);
      }
    });
  }

  goToRedemptions(): void {
    this.router.navigate(['/redemptions']);
  }

  goToRecognize(): void {
    this.router.navigate(['/manager/recognize']);
  }

  // placeholder image if no imageUrl
  getImage(item: RewardItem): string {
    return item.imageUrl || '';
  }

  hasImage(item: RewardItem): boolean {
    return !!item.imageUrl;
  }

  // color band based on bytes cost
  getBytesColor(bytes: number): string {
    if (bytes <= 500)  return 'text-emerald-600 bg-emerald-50';
    if (bytes <= 1500) return 'text-amber-600 bg-amber-50';
    return 'text-red-600 bg-red-50';
  }
}
