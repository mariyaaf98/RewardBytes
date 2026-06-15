import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { ModalComponent } from '../../../shared/components/modal/modal';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';
import { RewardCategoryService } from '../../../core/services/reward-category';
import { RewardCategory, CreateRewardCategoryRequest } from '../../../core/models/reward-category';

@Component({
  selector: 'app-reward-category-management',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent, ModalComponent, FormsModule],
  templateUrl: './reward-category-management.html',
  styleUrl: './reward-category-management.css'
})
export class RewardCategoryManagementComponent implements OnInit {

  private readonly service = inject(RewardCategoryService);

  adminMenu = ADMIN_MENU;

  categories: RewardCategory[] = [];
  searchText = '';
  selectedStatus = 'all';

  // Modal state
  isModalOpen = false;
  isEditMode = false;
  selectedId = '';

  // Form fields
  name = '';
  description = '';
  bytes: number | null = null;

  // Validation errors
  nameError = '';
  descriptionError = '';
  bytesError = '';

  // Inline alert
  alertMessage = '';
  alertType: 'success' | 'error' = 'success';
  showAlert = false;

  // Delete confirm modal
  showDeleteModal = false;
  categoryToDelete: RewardCategory | null = null;

  // Error modal
  errorMessage = '';
  showErrorModal = false;

  ngOnInit(): void {
    this.loadCategories();
  }

  loadCategories(): void {
    this.service.getRewardCategories().subscribe({
      next: (res) => { this.categories = res; },
      error: (err) => {
        this.showInlineAlert('Failed to load reward categories.', 'error');
        console.error(err);
      }
    });
  }

  get filteredCategories(): RewardCategory[] {
    let filtered = this.categories;

    // Search filter
    if (this.searchText.trim()) {
      const q = this.searchText.toLowerCase();
      filtered = filtered.filter(c =>
        c.name.toLowerCase().includes(q) ||
        c.description?.toLowerCase().includes(q)
      );
    }

    // Status filter
    if (this.selectedStatus === 'active') {
      filtered = filtered.filter(c => c.isActive);
    } else if (this.selectedStatus === 'inactive') {
      filtered = filtered.filter(c => !c.isActive);
    }

    return filtered;
  }

  openAddModal(): void {
    this.isEditMode = false;
    this.selectedId = '';
    this.name = '';
    this.description = '';
    this.bytes = null;
    this.clearErrors();
    this.isModalOpen = true;
  }

  openEditModal(cat: RewardCategory): void {
    this.isEditMode = true;
    this.selectedId = cat.id;
    this.name = cat.name;
    this.description = cat.description ?? '';
    this.bytes = cat.bytes;
    this.clearErrors();
    this.isModalOpen = true;
  }

  openDeleteModal(cat: RewardCategory): void {
    this.categoryToDelete = cat;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.categoryToDelete = null;
  }

  save(): void {
    if (!this.validate()) return;

    const payload: CreateRewardCategoryRequest = {
      name: this.name.trim(),
      description: this.description.trim(),
      bytes: this.bytes!
    };

    if (this.isEditMode) {
      this.service.updateRewardCategory(this.selectedId, payload).subscribe({
        next: () => {
          this.isModalOpen = false;
          this.resetForm();
          this.loadCategories();
          this.showInlineAlert(`Category "${payload.name}" updated successfully.`, 'success');
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.service.createRewardCategory(payload).subscribe({
        next: () => {
          this.isModalOpen = false;
          this.resetForm();
          this.loadCategories();
          this.showInlineAlert(`Category "${payload.name}" created successfully.`, 'success');
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  confirmDelete(): void {
    if (!this.categoryToDelete) return;

    this.service.deleteRewardCategory(this.categoryToDelete.id).subscribe({
      next: () => {
        this.showInlineAlert(`Category "${this.categoryToDelete!.name}" deleted.`, 'success');
        this.closeDeleteModal();
        this.loadCategories();
      },
      error: (err) => {
        this.closeDeleteModal();
        this.handleError(err);
      }
    });
  }

  getInitial(name: string): string {
    return name?.charAt(0)?.toUpperCase() ?? '?';
  }

  private validate(): boolean {
    this.clearErrors();
    let valid = true;

    // Name — required, unique (client-side duplicate check), max 100
    if (!this.name.trim()) {
      this.nameError = 'Category name is required.';
      valid = false;
    } else if (this.name.trim().length > 100) {
      this.nameError = 'Category name must not exceed 100 characters.';
      valid = false;
    } else {
      const duplicate = this.categories.some(c =>
        c.name.trim().toLowerCase() === this.name.trim().toLowerCase() &&
        c.id !== this.selectedId
      );
      if (duplicate) {
        this.nameError = 'A category with this name already exists.';
        valid = false;
      }
    }

    // Description — required, max 500
    if (!this.description.trim()) {
      this.descriptionError = 'Description is required.';
      valid = false;
    } else if (this.description.trim().length > 500) {
      this.descriptionError = 'Description must not exceed 500 characters.';
      valid = false;
    }

    // Bytes — required, must be > 0
    if (this.bytes === null || this.bytes === undefined || this.bytes === ('' as any)) {
      this.bytesError = 'Bytes value is required.';
      valid = false;
    } else if (isNaN(Number(this.bytes))) {
      this.bytesError = 'Bytes value must be a number.';
      valid = false;
    } else if (Number(this.bytes) <= 0) {
      this.bytesError = 'Bytes value must be greater than 0.';
      valid = false;
    }

    return valid;
  }

  private clearErrors(): void {
    this.nameError = '';
    this.descriptionError = '';
    this.bytesError = '';
  }

  private resetForm(): void {
    this.name = '';
    this.description = '';
    this.bytes = null;
    this.selectedId = '';
    this.isEditMode = false;
    this.nameError = '';
    this.descriptionError = '';
    this.bytesError = '';
  }

  private showInlineAlert(message: string, type: 'success' | 'error'): void {
    this.alertMessage = message;
    this.alertType = type;
    this.showAlert = true;
    setTimeout(() => { this.showAlert = false; }, 4000);
  }

  private handleError(error: any): void {
    console.error(error);
    if (error.error?.errors) {
      this.errorMessage = Object.values(error.error.errors).flat().join('\n');
    } else {
      this.errorMessage =
        error.error?.detail ?? error.error?.message ?? 'Something went wrong.';
    }
    this.showErrorModal = true;
  }

  closeErrorModal(): void {
    this.showErrorModal = false;
  }
}
