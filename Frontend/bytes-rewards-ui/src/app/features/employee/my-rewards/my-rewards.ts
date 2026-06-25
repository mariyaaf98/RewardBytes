import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

import { RewardService, RewardHistoryItem } from '../../../core/services/reward';
import { UserService } from '../../../core/services/user';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-my-rewards',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, SidebarComponent, TopbarComponent],
  templateUrl: './my-rewards.html',
  styleUrl: './my-rewards.css'
})
export class MyRewardsComponent implements OnInit {

  private readonly rewardService = inject(RewardService);
  private readonly userService   = inject(UserService);
  private readonly authService   = inject(AuthService);

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

  readonly rewards    = signal<RewardHistoryItem[]>([]);
  readonly isLoading  = signal(true);
  readonly error      = signal('');
  readonly searchText = signal('');

  readonly filtered = computed(() => {
    const q = this.searchText().toLowerCase().trim();
    if (!q) return this.rewards();
    return this.rewards().filter(r =>
      r.rewardCategoryName.toLowerCase().includes(q) ||
      r.awardedBy.toLowerCase().includes(q) ||
      r.reason.toLowerCase().includes(q)
    );
  });

  readonly totalBytes = computed(() =>
    this.rewards().reduce((sum, r) => sum + r.bytes, 0)
  );

  // The single highest-bytes reward (shown in the hero banner + "Best" badge)
  readonly bestReward = computed(() => {
    if (!this.rewards().length) return null;
    return this.rewards().reduce((best, r) => r.bytes > best.bytes ? r : best);
  });

  ngOnInit(): void {
    this.userService.getCurrentUser().subscribe({
      next: user => {
        this.rewardService.getRewardHistory(user.id).subscribe({
          next: data => {
            // Most recent first
            this.rewards.set([...data].sort(
              (a, b) => new Date(b.awardedAt).getTime() - new Date(a.awardedAt).getTime()
            ));
            this.isLoading.set(false);
          },
          error: err => {
            this.error.set(err.error?.detail ?? 'Failed to load rewards.');
            this.isLoading.set(false);
          }
        });
      },
      error: () => {
        this.error.set('Could not load your profile.');
        this.isLoading.set(false);
      }
    });
  }

  getCategoryIcon(category: string): string {
    const c = category.toLowerCase();
    if (c.includes('excellen')) return '🌟';
    if (c.includes('innovat'))  return '💡';
    if (c.includes('team'))     return '🤝';
    if (c.includes('leader'))   return '🏆';
    if (c.includes('perform'))  return '🚀';
    if (c.includes('help'))     return '🙌';
    return '🏅';
  }

  getBytesColor(bytes: number): string {
    if (bytes >= 1000) return 'text-amber-600 bg-amber-50 border-amber-200';
    if (bytes >= 500)  return 'text-emerald-600 bg-emerald-50 border-emerald-200';
    return 'text-blue-600 bg-blue-50 border-blue-200';
  }

  formatDate(dateStr: string): string {
    return new Date(dateStr).toLocaleDateString('en-GB', {
      day: '2-digit', month: 'short', year: 'numeric'
    });
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
