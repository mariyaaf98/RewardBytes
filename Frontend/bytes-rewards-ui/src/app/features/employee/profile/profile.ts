import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';

import { UserService } from '../../../core/services/user';
import { WalletService } from '../../../core/services/wallet';
import { AuthService } from '../../../core/services/auth';
import { AppreciationService } from '../../../core/services/appreciation';

import { CurrentUser } from '../../../core/models/user.model';

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, TopbarComponent],
  templateUrl: './profile.html',
  styleUrl: './profile.css'
})
export class ProfileComponent implements OnInit {

  private readonly userService         = inject(UserService);
  private readonly walletService       = inject(WalletService);
  private readonly authService         = inject(AuthService);
  private readonly appreciationService = inject(AppreciationService);
  private readonly router              = inject(Router);

  readonly employeeMenu = EMPLOYEE_MENU;

  // ── View state ─────────────────────────────────────────────────
  readonly profile        = signal<CurrentUser | null>(null);
  readonly availableBytes = signal<number | null>(null);
  readonly totalEarned    = signal<number>(0);
  readonly receivedCount  = signal<number>(0);
  readonly sentCount      = signal<number>(0);
  readonly isLoading      = signal(true);
  readonly error          = signal('');
  readonly role           = signal('');

  // ── Edit state ─────────────────────────────────────────────────
  readonly isEditing   = signal(false);
  readonly isSaving    = signal(false);
  readonly saveSuccess = signal(false);
  readonly saveError   = signal('');

  // form fields (only editable fields)
  editFirstName   = '';
  editLastName    = '';
  editPhoneNumber = '';
  editDesignation = '';

  // validation
  firstNameError   = '';
  lastNameError    = '';
  phoneError       = '';
  designationError = '';

  // ── Lifecycle ──────────────────────────────────────────────────
  ngOnInit(): void {
    this.role.set(this.authService.currentRole());
    this.loadProfile();
    this.loadAppreciationStats();
  }

  private loadProfile(): void {
    this.userService.getCurrentUser().subscribe({
      next: (user) => {
        this.profile.set(user);
        this.isLoading.set(false);
        this.loadWallet(user.id);
      },
      error: (err) => {
        this.error.set(err.error?.detail ?? 'Could not load profile.');
        this.isLoading.set(false);
      }
    });
  }

  private loadWallet(userId: string): void {
    this.walletService.getWallet(userId).subscribe({
      next: (res) => this.availableBytes.set(res.availableBytes),
      error: () => {}
    });
    this.walletService.getWalletLedger(userId).subscribe({
      next: (entries) => this.totalEarned.set(entries.reduce((s, e) => s + e.bytes, 0)),
      error: () => {}
    });
  }

  private loadAppreciationStats(): void {
    const name = this.authService.getUserName();
    this.appreciationService.getAppreciations().subscribe({
      next: (list) => {
        this.receivedCount.set(list.filter(a => a.toUserName === name).length);
        this.sentCount.set(list.filter(a => a.fromUserName === name).length);
      },
      error: () => {}
    });
  }

  // ── Edit ───────────────────────────────────────────────────────
  openEdit(): void {
    const p = this.profile();
    if (!p) return;
    this.editFirstName   = p.firstName;
    this.editLastName    = p.lastName;
    this.editPhoneNumber = p.phoneNumber;
    this.editDesignation = p.designation;
    this.clearErrors();
    this.saveError.set('');
    this.saveSuccess.set(false);
    this.isEditing.set(true);
  }

  cancelEdit(): void {
    this.isEditing.set(false);
    this.clearErrors();
  }

  saveEdit(): void {
    if (!this.validate()) return;

    const p = this.profile();
    if (!p) return;

    this.isSaving.set(true);
    this.saveError.set('');

    this.userService.updateUser(p.id, {
      firstName:   this.editFirstName.trim(),
      lastName:    this.editLastName.trim(),
      phoneNumber: this.editPhoneNumber.trim(),
      designation: this.editDesignation.trim(),
      role:        this.role(),
      departmentId: p.departmentId
    }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isEditing.set(false);
        this.saveSuccess.set(true);
        setTimeout(() => this.saveSuccess.set(false), 3500);
        // refresh profile
        this.loadProfile();
      },
      error: (err) => {
        this.isSaving.set(false);
        this.saveError.set(err.error?.detail ?? err.error?.message ?? 'Could not save changes.');
      }
    });
  }

  private validate(): boolean {
    this.clearErrors();
    let ok = true;

    if (!this.editFirstName.trim()) {
      this.firstNameError = 'First name is required.'; ok = false;
    }
    if (!this.editLastName.trim()) {
      this.lastNameError = 'Last name is required.'; ok = false;
    }
    if (this.editPhoneNumber.trim() && !/^\d{10}$/.test(this.editPhoneNumber.trim())) {
      this.phoneError = 'Enter a valid 10-digit phone number.'; ok = false;
    }
    if (!this.editDesignation.trim()) {
      this.designationError = 'Designation is required.'; ok = false;
    }
    return ok;
  }

  private clearErrors(): void {
    this.firstNameError = this.lastNameError = this.phoneError = this.designationError = '';
  }

  // ── Helpers ────────────────────────────────────────────────────
  getInitials(user: CurrentUser): string {
    return `${user.firstName[0] ?? ''}${user.lastName[0] ?? ''}`.toUpperCase();
  }

  getRoleBadgeStyle(): string {
    const r = this.role();
    if (r === 'admin')   return 'bg-purple-100 text-purple-700';
    if (r === 'manager') return 'bg-blue-100 text-blue-700';
    return 'bg-emerald-100 text-emerald-700';
  }

  goToWallet(): void        { this.router.navigate(['/wallet']); }
  goToAppreciations(): void { this.router.navigate(['/employee/appreciations/history']); }
}
