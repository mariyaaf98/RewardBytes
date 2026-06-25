import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { ModalComponent } from '../../../shared/components/modal/modal';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';
import { DesignationService, CreateDesignationRequest } from '../../../core/services/designation';
import { Designation } from '../../../core/models/lookup';

@Component({
  selector: 'app-designation-management',
  standalone: true,
  imports: [
    CommonModule,
    SidebarComponent,
    TopbarComponent,
    ModalComponent,
    FormsModule
  ],
  templateUrl: './designation-management.html',
  styleUrl: './designation-management.css'
})
export class DesignationManagementComponent implements OnInit {

  private readonly designationService = inject(DesignationService);

  adminMenu = ADMIN_MENU;

  designations: Designation[] = [];

  searchText = '';
  selectedStatus = 'all';

  isModalOpen = false;
  isEditMode = false;
  selectedDesignationId = '';

  // Form fields
  name = '';
  description = '';

  nameError = '';

  // Inline alert
  alertMessage = '';
  alertType: 'success' | 'error' = 'success';
  showAlert = false;

  // Error modal
  errorMessage = '';
  showErrorModal = false;

  // Delete confirm modal
  showDeleteModal = false;
  designationToDelete: Designation | null = null;

  ngOnInit(): void {
    this.loadDesignations();
  }

  loadDesignations(): void {
    this.designationService.getDesignations().subscribe({
      next: (res) => { this.designations = res; },
      error: (err) => {
        this.showInlineAlert('Failed to load designations.', 'error');
        console.error(err);
      }
    });
  }

  get filteredDesignations(): Designation[] {
    let filtered = this.designations;

    if (this.searchText.trim()) {
      const q = this.searchText.toLowerCase();
      filtered = filtered.filter(d =>
        d.name.toLowerCase().includes(q) ||
        d.description?.toLowerCase().includes(q)
      );
    }

    if (this.selectedStatus === 'active') {
      filtered = filtered.filter(d => d.isActive);
    } else if (this.selectedStatus === 'inactive') {
      filtered = filtered.filter(d => !d.isActive);
    }

    return filtered;
  }

  openAddModal(): void {
    this.isEditMode = false;
    this.selectedDesignationId = '';
    this.name = '';
    this.description = '';
    this.nameError = '';
    this.isModalOpen = true;
  }

  openEditModal(designation: Designation): void {
    this.isEditMode = true;
    this.selectedDesignationId = designation.id;
    this.name = designation.name;
    this.description = designation.description ?? '';
    this.nameError = '';
    this.isModalOpen = true;
  }

  openDeleteModal(designation: Designation): void {
    this.designationToDelete = designation;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.designationToDelete = null;
  }

  save(): void {
    this.nameError = '';

    if (!this.name.trim()) {
      this.nameError = 'Designation name is required.';
      return;
    }

    const payload: CreateDesignationRequest = {
      name: this.name.trim(),
      description: this.description.trim()
    };

    if (this.isEditMode) {
      this.designationService.updateDesignation(this.selectedDesignationId, payload).subscribe({
        next: () => {
          this.isModalOpen = false;
          this.resetForm();
          this.loadDesignations();
          this.showInlineAlert(`Designation "${payload.name}" updated successfully.`, 'success');
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.designationService.createDesignation(payload).subscribe({
        next: () => {
          this.isModalOpen = false;
          this.resetForm();
          this.loadDesignations();
          this.showInlineAlert(`Designation "${payload.name}" created successfully.`, 'success');
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  confirmDelete(): void {
    if (!this.designationToDelete) return;

    this.designationService.deleteDesignation(this.designationToDelete.id).subscribe({
      next: () => {
        this.showInlineAlert(`Designation "${this.designationToDelete!.name}" deleted.`, 'success');
        this.closeDeleteModal();
        this.loadDesignations();
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

  resetForm(): void {
    this.name = '';
    this.description = '';
    this.nameError = '';
    this.selectedDesignationId = '';
    this.isEditMode = false;
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
