import { Component, computed, inject, OnInit, signal } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { RewardCategoryPickerModalComponent } from '../../../shared/components/reward-category-picker-modal/reward-category-picker-modal';
import { RewardCategoryCardComponent } from '../../../shared/components/reward-category-card/reward-category-card';

import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import { UserService } from '../../../core/services/user';
import { RewardCategoryService } from '../../../core/services/reward-category';
import { RewardService, CreateRewardRequest, RewardResponse } from '../../../core/services/reward';
import { UserLookup } from '../../../core/models/lookup';
import { RewardCategory } from '../../../core/models/reward-category';

@Component({
  selector: 'app-assign-reward',
  standalone: true,
  imports: [
    CommonModule,
    SidebarComponent,
    TopbarComponent,
    FormsModule,
    DatePipe,
    RewardCategoryPickerModalComponent,
    RewardCategoryCardComponent,
  ],
  templateUrl: './assign-reward.html',
  styleUrl: './assign-reward.css',
})
export class AssignRewardComponent implements OnInit {
  private readonly userService     = inject(UserService);
  private readonly categoryService = inject(RewardCategoryService);
  private readonly rewardService   = inject(RewardService);

  readonly managerMenu = MANAGER_MENU;

  // ── Data signals ──────────────────────────────────────────────
  readonly allUsers      = signal<UserLookup[]>([]);
  readonly allCategories = signal<RewardCategory[]>([]);
  readonly recentRewards = signal<RewardResponse[]>([]);

  // ── Form signals ──────────────────────────────────────────────
  readonly userSearchText     = signal('');
  readonly selectedUserId     = signal('');
  readonly selectedCategory   = signal<RewardCategory | null>(null);
  readonly reason             = signal('');

  // ── UI state signals ──────────────────────────────────────────
  readonly showPickerModal  = signal(false);
  readonly isSubmitting     = signal(false);
  readonly isLoadingHistory = signal(false);
  readonly showSuccess      = signal(false);
  readonly errorMessage     = signal('');
  readonly showErrorModal   = signal(false);

  // ── Validation error signals ──────────────────────────────────
  readonly userError     = signal('');
  readonly categoryError = signal('');
  readonly reasonError   = signal('');

  // ── Derived signals ───────────────────────────────────────────
  readonly filteredUsers = computed(() => {
    const q = this.userSearchText().toLowerCase();
    if (!q) return this.allUsers();
    return this.allUsers().filter(u => u.fullName.toLowerCase().includes(q));
  });

  readonly showUserDropdown = computed(
    () =>
      this.filteredUsers().length > 0 &&
      !!this.userSearchText() &&
      !this.selectedUserId()
  );

  readonly selectedUserInitials = computed(() => {
    const u = this.allUsers().find(u => u.id === this.selectedUserId());
    if (!u) return '??';
    return u.fullName.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  });

  readonly activeCategories = computed(() =>
    this.allCategories().filter(c => c.isActive)
  );

  // ── Lifecycle ─────────────────────────────────────────────────
  ngOnInit(): void {
    this.loadUsers();
    this.loadCategories();
    this.loadHistory();
  }

  // ── Data loaders ──────────────────────────────────────────────
  loadUsers(): void {
    this.userService.getUserLookup().subscribe({
      next: users => this.allUsers.set(users),
      error: err  => console.error(err),
    });
  }

  loadCategories(): void {
    this.categoryService.getRewardCategories().subscribe({
      next: cats => this.allCategories.set(cats),
      error: err  => console.error(err),
    });
  }

  loadHistory(): void {
    this.isLoadingHistory.set(true);
    this.rewardService.getRewards().subscribe({
      next: rewards => {
        this.recentRewards.set([...rewards].reverse().slice(0, 10));
        this.isLoadingHistory.set(false);
      },
      error: () => this.isLoadingHistory.set(false),
    });
  }

  // ── User autocomplete ─────────────────────────────────────────
  onUserSearchInput(value: string): void {
    this.userSearchText.set(value);
    this.selectedUserId.set(''); // clear selection while typing
    this.userError.set('');
  }

  selectUser(user: UserLookup): void {
    this.selectedUserId.set(user.id);
    this.userSearchText.set(user.fullName);
    this.userError.set('');
  }

  // ── Category picker ───────────────────────────────────────────
  openPicker(): void {
    this.showPickerModal.set(true);
  }

  onCategorySelected(cat: RewardCategory): void {
    this.selectedCategory.set(cat);
    this.showPickerModal.set(false);
    this.categoryError.set('');
  }

  onPickerClosed(): void {
    this.showPickerModal.set(false);
  }

  clearCategory(): void {
    this.selectedCategory.set(null);
  }

  // ── Validation ────────────────────────────────────────────────
  private validate(): boolean {
    this.userError.set('');
    this.categoryError.set('');
    this.reasonError.set('');
    let valid = true;

    if (!this.selectedUserId()) {
      this.userError.set('Please select an employee.');
      valid = false;
    }

    if (!this.selectedCategory()) {
      this.categoryError.set('Please select a reward category.');
      valid = false;
    }

    const r = this.reason().trim();
    if (!r) {
      this.reasonError.set('Reason is required.');
      valid = false;
    } else if (r.length < 10) {
      this.reasonError.set('Reason must be at least 10 characters.');
      valid = false;
    } else if (r.length > 500) {
      this.reasonError.set('Reason must not exceed 500 characters.');
      valid = false;
    }

    return valid;
  }

  // ── Submit ────────────────────────────────────────────────────
  submit(): void {
    if (!this.validate()) return;

    this.isSubmitting.set(true);

    const payload: CreateRewardRequest = {
      toUserId:         this.selectedUserId(),
      rewardCategoryId: this.selectedCategory()!.id,
      reason:           this.reason().trim(),
    };

    this.rewardService.createReward(payload).subscribe({
      next: () => {
        this.isSubmitting.set(false);
        this.showSuccess.set(true);
        this.resetForm();
        this.loadHistory();
        setTimeout(() => this.showSuccess.set(false), 4000);
      },
      error: err => {
        this.isSubmitting.set(false);
        this.errorMessage.set(
          err.error?.detail ?? err.error?.message ?? 'Something went wrong.'
        );
        this.showErrorModal.set(true);
      },
    });
  }

  // ── Reset ─────────────────────────────────────────────────────
  resetForm(): void {
    this.selectedUserId.set('');
    this.userSearchText.set('');
    this.selectedCategory.set(null);
    this.reason.set('');
    this.userError.set('');
    this.categoryError.set('');
    this.reasonError.set('');
  }

  closeErrorModal(): void {
    this.showErrorModal.set(false);
  }

  // ── Helpers ───────────────────────────────────────────────────
  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
