import { Component, inject, OnInit, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

import { UserService } from '../../../core/services/user';
import { WalletService } from '../../../core/services/wallet';
import { AuthService } from '../../../core/services/auth';
import { AppreciationService } from '../../../core/services/appreciation';
import { UploadService } from '../../../core/services/upload';

import { CurrentUser } from '../../../core/models/user.model';

type ActiveSection = 'details' | 'password' | 'avatar';

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
  private readonly uploadService       = inject(UploadService);
  private readonly router              = inject(Router);

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

  // ── View state ─────────────────────────────────────────────────
  readonly profile        = signal<CurrentUser | null>(null);
  readonly availableBytes = signal<number | null>(null);
  readonly totalEarned    = signal<number>(0);
  readonly receivedCount  = signal<number>(0);
  readonly sentCount      = signal<number>(0);
  readonly isLoading      = signal(true);
  readonly error          = signal('');
  readonly role           = signal('');

  // active right-column section
  activeSection: ActiveSection = 'details';

  // ── Profile edit ───────────────────────────────────────────────
  readonly isEditing   = signal(false);
  readonly isSaving    = signal(false);
  readonly saveSuccess = signal('');
  readonly saveError   = signal('');

  editFirstName   = '';
  editLastName    = '';
  editPhoneNumber = '';

  firstNameError   = '';
  lastNameError    = '';
  phoneError       = '';

  // ── Change password ────────────────────────────────────────────
  currentPassword  = '';
  newPassword      = '';
  confirmPassword  = '';
  currentPasswordError = '';
  newPasswordError = '';
  confirmError     = '';
  isSavingPassword = false;

  // eye toggle state for password fields
  showCurrentPw = false;
  showNewPw     = false;
  showConfirmPw = false;

  // ── Profile image ──────────────────────────────────────────────
  newImageUrl      = '';
  imageUrlError    = '';
  isSavingImage    = false;
  imagePreviewUrl  = '';
  isDraggingOver   = false;
  private _pendingFile: File | null = null;

  // expose for template binding
  get pendingFile(): File | null { return this._pendingFile; }

  onFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file  = input.files?.[0];
    if (file) this.prepareImage(file);
    input.value = '';
  }

  onDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOver = true;
  }

  onDragLeave(): void { this.isDraggingOver = false; }

  onDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOver = false;
    const file = event.dataTransfer?.files?.[0];
    if (file && file.type.startsWith('image/')) this.prepareImage(file);
  }

  private prepareImage(file: File): void {
    if (file.size > 2 * 1024 * 1024) {
      this.imageUrlError = 'Image must be under 2 MB.';
      return;
    }
    this.imageUrlError  = '';
    this._pendingFile   = file;
    // show local preview immediately
    const reader = new FileReader();
    reader.onload = e => { this.imagePreviewUrl = e.target?.result as string; };
    reader.readAsDataURL(file);
  }

  saveImage(): void {
    this.imageUrlError = '';
    this.saveError.set('');
    this.saveSuccess.set('');

    if (!this._pendingFile) {
      this.imageUrlError = 'Please select an image first.'; return;
    }

    this.isSavingImage = true;

    // Step 1 — upload file to server, get back a URL
    this.uploadService.uploadImage(this._pendingFile).subscribe({
      next: (url) => {
        // Step 2 — save the URL to the user's profile
        this.userService.updateProfileImage(url).subscribe({
          next: () => {
            this.isSavingImage  = false;
            this._pendingFile   = null;
            this.newImageUrl    = url;
            this.saveSuccess.set('Profile photo updated.');
            setTimeout(() => this.saveSuccess.set(''), 3500);
            this.loadProfile();
          },
          error: err => {
            this.isSavingImage = false;
            this.saveError.set(err.error?.detail ?? 'Could not save photo URL.');
          }
        });
      },
      error: err => {
        this.isSavingImage = false;
        const msg = err.error?.errors?.[0]?.message ?? err.error?.detail ?? 'Upload failed.';
        this.imageUrlError = msg;
      }
    });
  }

  removeImage(): void {
    this._pendingFile   = null;
    this.newImageUrl    = '';
    this.imagePreviewUrl = '';
    this.imageUrlError  = '';
    this.isSavingImage  = true;
    this.userService.updateProfileImage('').subscribe({
      next: () => { this.isSavingImage = false; this.loadProfile(); },
      error: () => { this.isSavingImage = false; }
    });
  }
  ngOnInit(): void {
    this.role.set(this.authService.currentRole());
    this.loadProfile();
    this.loadAppreciationStats();
  }

  private loadProfile(): void {
    this.userService.getCurrentUser().subscribe({
      next: user => {
        this.profile.set(user);
        this.newImageUrl     = user.profileImageUrl ?? '';
        this.imagePreviewUrl = user.profileImageUrl ?? '';
        this.isLoading.set(false);
        this.loadWallet(user.id);
      },
      error: err => {
        this.error.set(err.error?.detail ?? 'Could not load profile.');
        this.isLoading.set(false);
      }
    });
  }

  private loadWallet(userId: string): void {
    this.walletService.getWallet(userId).subscribe({
      next: r => this.availableBytes.set(r.availableBytes), error: () => {}
    });
    this.walletService.getWalletLedger(userId).subscribe({
      next: e => this.totalEarned.set(e.reduce((s, i) => s + i.bytes, 0)), error: () => {}
    });
  }

  private loadAppreciationStats(): void {
    this.userService.getCurrentUser().subscribe({
      next: currentUser => {
        this.appreciationService.getAppreciations().subscribe({
          next: list => {
            this.receivedCount.set(list.filter(a => a.toUserId   === currentUser.id).length);
            this.sentCount.set(    list.filter(a => a.fromUserId === currentUser.id).length);
          },
          error: () => {}
        });
      },
      error: () => {}
    });
  }

  // ── Section nav ────────────────────────────────────────────────
  setSection(s: ActiveSection): void {
    this.activeSection = s;
    this.saveSuccess.set('');
    this.saveError.set('');
    if (s === 'details' && !this.isEditing()) this.isEditing.set(false);
  }

  // ── Profile details edit ───────────────────────────────────────
  openEdit(): void {
    const p = this.profile();
    if (!p) return;
    this.editFirstName   = p.firstName;
    this.editLastName    = p.lastName;
    this.editPhoneNumber = p.phoneNumber;
    this.clearProfileErrors();
    this.saveError.set('');
    this.saveSuccess.set('');
    this.isEditing.set(true);
  }

  cancelEdit(): void { this.isEditing.set(false); this.clearProfileErrors(); }

  saveEdit(): void {
    if (!this.validateProfile()) return;
    const p = this.profile();
    if (!p) return;

    this.isSaving.set(true);
    this.saveError.set('');

    this.userService.updateCurrentUser({
      firstName:   this.editFirstName.trim(),
      lastName:    this.editLastName.trim(),
      phoneNumber: this.editPhoneNumber.trim()
    }).subscribe({
      next: () => {
        this.isSaving.set(false);
        this.isEditing.set(false);
        this.saveSuccess.set('Profile updated successfully.');
        setTimeout(() => this.saveSuccess.set(''), 3500);
        this.loadProfile();
      },
      error: err => {
        this.isSaving.set(false);
        this.saveError.set(err.error?.detail ?? 'Could not save changes.');
      }
    });
  }

  private validateProfile(): boolean {
    this.clearProfileErrors(); let ok = true;
    if (!this.editFirstName.trim())  { this.firstNameError = 'Required.'; ok = false; }
    if (!this.editLastName.trim())   { this.lastNameError  = 'Required.'; ok = false; }
    if (this.editPhoneNumber.trim() && !/^\d{10}$/.test(this.editPhoneNumber.trim()))
      { this.phoneError = '10-digit number required.'; ok = false; }
    return ok;
  }

  private clearProfileErrors(): void {
    this.firstNameError = this.lastNameError = this.phoneError = '';
  }

  // ── Change password ────────────────────────────────────────────
  savePassword(): void {
    this.currentPasswordError = '';
    this.newPasswordError     = '';
    this.confirmError         = '';
    this.saveError.set('');
    this.saveSuccess.set('');
    let ok = true;

    if (!this.currentPassword)
      { this.currentPasswordError = 'Current password is required.'; ok = false; }
    if (!this.newPassword || this.newPassword.length < 8)
      { this.newPasswordError = 'Password must be at least 8 characters.'; ok = false; }
    if (this.newPassword === this.currentPassword && this.newPassword)
      { this.newPasswordError = 'New password must be different from current.'; ok = false; }
    if (this.newPassword !== this.confirmPassword)
      { this.confirmError = 'Passwords do not match.'; ok = false; }
    if (!ok) return;

    this.isSavingPassword = true;
    this.userService.changePassword(this.currentPassword, this.newPassword).subscribe({
      next: () => {
        this.isSavingPassword  = false;
        this.currentPassword   = '';
        this.newPassword       = '';
        this.confirmPassword   = '';
        this.saveSuccess.set('Password changed successfully.');
        setTimeout(() => this.saveSuccess.set(''), 4000);
      },
      error: err => {
        this.isSavingPassword = false;
        const msg = err.error?.detail ?? err.error?.message ?? '';
        // Surface the current-password error clearly
        if (msg.toLowerCase().includes('incorrect') || err.status === 401) {
          this.currentPasswordError = 'Current password is incorrect.';
        } else {
          this.saveError.set(msg || 'Could not change password.');
        }
      }
    });
  }

  // ── Profile image ──────────────────────────────────────────────
  onImageUrlChange(url: string): void {
    this.newImageUrl     = url;
    this.imagePreviewUrl = url;  // live preview
    this.imageUrlError   = '';
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

  goToWallet():        void { this.router.navigate(['/wallet']); }
  goToAppreciations(): void { this.router.navigate(['/employee/appreciations/history']); }
}
