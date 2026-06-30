import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { ModalComponent } from '../../../shared/components/modal/modal';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';
import { RewardCatalogService } from '../../../core/services/reward-catalog';
import { UploadService } from '../../../core/services/upload';
import { RewardItem } from '../../../core/models/reward-item';

type ViewMode = 'table' | 'grid';

@Component({
  selector: 'app-reward-item-management',
  standalone: true,
  imports: [CommonModule, FormsModule, SidebarComponent, TopbarComponent, ModalComponent],
  templateUrl: './reward-item-management.html',
  styleUrl: './reward-item-management.css'
})
export class RewardItemManagementComponent implements OnInit {

  private readonly service      = inject(RewardCatalogService);
  private readonly uploadService = inject(UploadService);

  readonly adminMenu = ADMIN_MENU;

  // ── State ─────────────────────────────────────────────────────
  readonly items      = signal<RewardItem[]>([]);
  readonly isLoading  = signal(true);
  readonly searchText = signal('');
  readonly viewMode   = signal<ViewMode>('table');

  readonly selectedStatus = signal<'all' | 'active' | 'inactive'>('all');

  // Panel (slide-over for add/edit)
  isPanelOpen = false;
  isEditMode  = false;
  selectedId  = '';

  // Form fields
  name          = '';
  productCode   = '';
  description   = '';
  requiredBytes: number | null = null;
  imageUrl      = '';
  isActive      = true;
  isSaving      = false;

  // Validation
  nameError          = '';
  productCodeError   = '';
  requiredBytesError = '';

  // Inline upload feedback (shown inside the modal)
  uploadError   = '';
  uploadSuccess = '';

  // Feedback
  alertMessage = '';
  alertType: 'success' | 'error' = 'success';
  showAlert    = false;

  // Delete
  showDeleteModal = false;
  itemToDelete: RewardItem | null = null;

  // Error
  errorMessage   = '';
  showErrorModal = false;

  // ── Derived ───────────────────────────────────────────────────
  readonly filtered = computed(() => {
    const q      = this.searchText().toLowerCase().trim();
    const status = this.selectedStatus();
    let list     = this.items();

    if (status === 'active')   list = list.filter(i => i.isActive);
    if (status === 'inactive') list = list.filter(i => !i.isActive);

    if (!q) return list;
    return list.filter(i =>
      i.name.toLowerCase().includes(q) ||
      i.productCode.toLowerCase().includes(q) ||
      i.description.toLowerCase().includes(q)
    );
  });

  readonly activeCount   = computed(() => this.items().filter(i => i.isActive).length);
  readonly inactiveCount = computed(() => this.items().filter(i => !i.isActive).length);
  readonly totalBytes    = computed(() =>
    this.items().reduce((s, i) => s + i.requiredBytes, 0)
  );

  // ── Lifecycle ─────────────────────────────────────────────────
  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.service.getRewardItems().subscribe({
      next:  d => { this.items.set(d); this.isLoading.set(false); },
      error: e => { this.handleError(e); this.isLoading.set(false); }
    });
  }

  onSearch(v: string): void { this.searchText.set(v); }
  setView(v: ViewMode): void { this.viewMode.set(v); }

  // ── Panel ─────────────────────────────────────────────────────
  openAdd(): void {
    this.isEditMode = false; this.selectedId = '';
    this.name = ''; this.productCode = ''; this.description = '';
    this.requiredBytes = null; this.imageUrl = ''; this.imagePreview = ''; this.isActive = true;
    this.clearErrors();
    this.uploadError = ''; this.uploadSuccess = '';
    this.isPanelOpen = true;
  }

  openEdit(item: RewardItem): void {
    this.isEditMode = true; this.selectedId = item.id;
    this.name = item.name; this.productCode = item.productCode;
    this.description = item.description; this.requiredBytes = item.requiredBytes;
    this.imageUrl = item.imageUrl; this.imagePreview = item.imageUrl; this.isActive = item.isActive;
    this.clearErrors();
    this.uploadError = ''; this.uploadSuccess = '';
    this.isPanelOpen = true;
  }

  closePanel(): void {
    this.isPanelOpen       = false;
    this._pendingImageFile = null;
  }

  save(): void {
    if (!this.validate()) return;
    this.isSaving = true;
    this.uploadThenSave();
  }

  private doSave(): void {
    const payload = {
      productCode:   this.productCode.trim(),
      name:          this.name.trim(),
      description:   this.description.trim(),
      requiredBytes: this.requiredBytes!,
      isActive:      this.isActive,
      imageUrl:      this.imageUrl
    };

    const call = this.isEditMode
      ? this.service.updateRewardItem(this.selectedId, payload)
      : this.service.createRewardItem(payload);

    call.subscribe({
      next: () => {
        this.isSaving = false; this.isPanelOpen = false;
        this.load();
        this.toast(`"${payload.name}" ${this.isEditMode ? 'updated' : 'created'} successfully.`, 'success');
      },
      error: err => { this.isSaving = false; this.handleError(err); }
    });
  }

  // ── Delete ────────────────────────────────────────────────────
  openDelete(item: RewardItem): void { this.itemToDelete = item; this.showDeleteModal = true; }
  closeDelete(): void { this.itemToDelete = null; this.showDeleteModal = false; }

  confirmDelete(): void {
    if (!this.itemToDelete) return;
    this.service.deleteRewardItem(this.itemToDelete.id).subscribe({
      next: () => {
        this.toast(`"${this.itemToDelete!.name}" deleted.`, 'success');
        this.closeDelete(); this.load();
      },
      error: err => { this.closeDelete(); this.handleError(err); }
    });
  }

  // ── Helpers ───────────────────────────────────────────────────
  private validate(): boolean {
    this.clearErrors(); let ok = true;
    if (!this.name.trim())        { this.nameError = 'Name is required.'; ok = false; }
    if (!this.productCode.trim()) { this.productCodeError = 'Product code is required.'; ok = false; }
    if (!this.requiredBytes || this.requiredBytes <= 0)
      { this.requiredBytesError = 'Enter a value greater than 0.'; ok = false; }
    return ok;
  }

  private clearErrors(): void {
    this.nameError = this.productCodeError = this.requiredBytesError = '';
  }

  private toast(msg: string, type: 'success' | 'error'): void {
    this.alertMessage = msg; this.alertType = type; this.showAlert = true;
    setTimeout(() => this.showAlert = false, 4000);
  }

  private handleError(err: any): void {
    this.errorMessage = err.error?.detail ?? err.error?.message ?? 'Something went wrong.';
    this.showErrorModal = true;
  }

  // ── Image upload ──────────────────────────────────────────────
  isDraggingOver  = false;
  imagePreview    = '';
  private _pendingImageFile: File | null = null;

  onImageFileSelected(event: Event): void {
    const input = event.target as HTMLInputElement;
    const file  = input.files?.[0];
    if (file) this.prepareImage(file);
    input.value = '';
  }

  onImageDragOver(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOver = true;
  }

  onImageDragLeave(): void { this.isDraggingOver = false; }

  onImageDrop(event: DragEvent): void {
    event.preventDefault();
    this.isDraggingOver = false;
    const file = event.dataTransfer?.files?.[0];
    if (file && file.type.startsWith('image/')) this.prepareImage(file);
  }

  private prepareImage(file: File): void {
    this._pendingImageFile = file;
    const reader = new FileReader();
    reader.onload = e => { this.imagePreview = e.target?.result as string; };
    reader.readAsDataURL(file);
  }

  clearImage(): void {
    this._pendingImageFile = null;
    this.imageUrl          = '';
    this.imagePreview      = '';
  }

  /** Called inside save() — uploads file first if pending, then saves item */
  private uploadThenSave(): void {
    if (!this._pendingImageFile) {
      this.doSave(); return;
    }

    this.uploadError   = '';
    this.uploadSuccess = '';

    this.uploadService.uploadImage(this._pendingImageFile).subscribe({
      next: url => {
        this.imageUrl          = url;
        this._pendingImageFile = null;
        this.imagePreview      = url;
        this.uploadSuccess     = '✓ Image uploaded successfully.';
        this.doSave();
      },
      error: err => {
        this.isSaving      = false;
        this.uploadError   = err.error?.errors?.[0]?.message
                          ?? err.error?.detail
                          ?? 'Image upload failed. Please try a different image.';
      }
    });
  }

  bytesClass(b: number): string {
    if (b <= 500)  return 'text-emerald-600 bg-emerald-50';
    if (b <= 1500) return 'text-amber-600 bg-amber-50';
    return 'text-red-600 bg-red-50';
  }

  getInitial(name: string): string {
    return name.charAt(0).toUpperCase();
  }

  closeErrorModal(): void {
    this.showErrorModal = false;
    this.errorMessage   = '';
  }
}
