import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { ModalComponent } from '../../../shared/components/modal/modal';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';
import { DepartmentService, CreateDepartmentRequest } from '../../../core/services/department';
import { Department } from '../../../core/models/lookup';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-department-management',
  standalone: true,
  imports: [
    CommonModule,
    SidebarComponent,
    TopbarComponent,
    ModalComponent,
    FormsModule
  ],
  templateUrl: './department-management.html',
  styleUrl: './department-management.css'
})
export class DepartmentManagementComponent implements OnInit {

  private readonly departmentService = inject(DepartmentService);

  adminMenu = ADMIN_MENU;

  departments: Department[] = [];

  searchText = '';
  selectedStatus = 'all';

  isModalOpen = false;

  isEditMode = false;

  selectedDepartmentId = '';

  // Form fields
  name = '';
  description = '';

  nameError = '';

  // Alert
  alertMessage = '';
  alertType: 'success' | 'error' = 'success';
  showAlert = false;

  // Error modal
  errorMessage = '';
  showErrorModal = false;

  // Delete confirm modal
  showDeleteModal = false;
  departmentToDelete: Department | null = null;

  ngOnInit(): void {
    this.loadDepartments();
  }

  loadDepartments(): void {
    this.departmentService.getDepartments().subscribe({
      next: (res) => {
        this.departments = res;
      },
      error: (err) => {
        this.showInlineAlert('Failed to load departments.', 'error');
        console.error(err);
      }
    });
  }

  get filteredDepartments(): Department[] {
    let filtered = this.departments;

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
    this.selectedDepartmentId = '';
    this.name = '';
    this.description = '';
    this.nameError = '';
    this.isModalOpen = true;
  }

  openEditModal(dept: Department): void {
    this.isEditMode = true;
    this.selectedDepartmentId = dept.id;
    this.name = dept.name;
    this.description = dept.description ?? '';
    this.nameError = '';
    this.isModalOpen = true;
  }

  openDeleteModal(dept: Department): void {
    this.departmentToDelete = dept;
    this.showDeleteModal = true;
  }

  closeDeleteModal(): void {
    this.showDeleteModal = false;
    this.departmentToDelete = null;
  }

  save(): void {
    this.nameError = '';

    if (!this.name.trim()) {
      this.nameError = 'Department name is required.';
      return;
    }

    const payload: CreateDepartmentRequest = {
      name: this.name.trim(),
      description: this.description.trim()
    };

    if (this.isEditMode) {
      this.departmentService.updateDepartment(this.selectedDepartmentId, payload).subscribe({
        next: () => {
          this.isModalOpen = false;
          this.resetForm();
          this.loadDepartments();
          this.showInlineAlert(`Department "${payload.name}" updated successfully.`, 'success');
        },
        error: (err) => this.handleError(err)
      });
    } else {
      this.departmentService.createDepartment(payload).subscribe({
        next: () => {
          this.isModalOpen = false;
          this.resetForm();
          this.loadDepartments();
          this.showInlineAlert(`Department "${payload.name}" created successfully.`, 'success');
        },
        error: (err) => this.handleError(err)
      });
    }
  }

  confirmDelete(): void {
    if (!this.departmentToDelete) return;

    this.departmentService.deleteDepartment(this.departmentToDelete.id).subscribe({
      next: () => {
        this.showInlineAlert(`Department "${this.departmentToDelete!.name}" deleted.`, 'success');
        this.closeDeleteModal();
        this.loadDepartments();
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
    this.selectedDepartmentId = '';
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
