// admin/librarians/librarians.ts
// Librarians page — Add, Edit/Update, Delete librarians

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-librarians',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './librarians.html',
  styleUrl: './librarians.css'
})
export class LibrariansComponent {

  // ── Modal control ─────────────────────────────────────────────
  showModal  = false;
  isEditMode = false;   // false = Add mode, true = Edit mode
  editingId  = 0;       // id of the librarian being edited

  // ── Toast notification ────────────────────────────────────────
  toastMessage = '';
  toastType    = '';

  // ── Form fields ───────────────────────────────────────────────
  form = {
    name:   '',
    email:  '',
    phone:  '',
    branch: '',
    status: 'Active'
  };

  // ── Librarians list (sample data) ─────────────────────────────
  librarians = [
    { id: 1, name: 'Mr. Hassan',  email: 'hassan@lib.com', phone: '012-1112222', branch: 'Main Branch', status: 'Active' },
    { id: 2, name: 'Ms. Priya',   email: 'priya@lib.com',  phone: '013-3334444', branch: 'East Wing',   status: 'Active' },
    { id: 3, name: 'Mr. Rajan',   email: 'rajan@lib.com',  phone: '014-5556666', branch: 'West Wing',   status: 'Active' },
    { id: 4, name: 'Ms. Siti',    email: 'siti@lib.com',   phone: '015-7778888', branch: 'North Block', status: 'Inactive' },
  ];

  // ── Branch options for dropdown ───────────────────────────────
  branches = [
    'Main Branch',
    'East Wing',
    'West Wing',
    'North Block',
    'South Block',
  ];

  // ── Open modal for ADD ────────────────────────────────────────
  openAddModal() {
    this.isEditMode = false;
    this.editingId  = 0;
    this.form = { name: '', email: '', phone: '', branch: '', status: 'Active' };
    this.showModal = true;
  }

  // ── Open modal for EDIT ───────────────────────────────────────
  openEditModal(lib: any) {
    this.isEditMode = true;
    this.editingId  = lib.id;

    // Fill form with selected librarian's data
    this.form = {
      name:   lib.name,
      email:  lib.email,
      phone:  lib.phone,
      branch: lib.branch,
      status: lib.status
    };

    this.showModal = true;
  }

  // ── Close modal ───────────────────────────────────────────────
  closeModal() {
    this.showModal = false;
  }

  // ── Save — Add or Update ──────────────────────────────────────
  saveLibrarian() {
    // Validate required fields
    if (!this.form.name || !this.form.email) {
      this.showToast('Please fill in Name and Email!', 'error');
      return;
    }

    if (this.isEditMode) {
      // ── UPDATE existing librarian ──
      const index = this.librarians.findIndex(l => l.id === this.editingId);
      if (index !== -1) {
        this.librarians[index] = {
          id:     this.editingId,
          name:   this.form.name,
          email:  this.form.email,
          phone:  this.form.phone,
          branch: this.form.branch,
          status: this.form.status
        };
      }
      this.showToast('Librarian updated successfully!', 'success');

    } else {
      // ── ADD new librarian ──
      const newLib = {
        id:     Date.now(),
        name:   this.form.name,
        email:  this.form.email,
        phone:  this.form.phone,
        branch: this.form.branch,
        status: this.form.status
      };
      this.librarians.push(newLib);
      this.showToast('Librarian created successfully!', 'success');
    }

    this.showModal = false;
  }

  // ── Delete librarian ──────────────────────────────────────────
  deleteLibrarian(id: number) {
    this.librarians = this.librarians.filter(l => l.id !== id);
    this.showToast('Librarian deleted.', 'error');
  }

  // ── Toast helper ──────────────────────────────────────────────
  showToast(message: string, type: string) {
    this.toastMessage = message;
    this.toastType    = type;
    setTimeout(() => { this.toastMessage = ''; }, 2500);
  }

  // ── Avatar initial letter ─────────────────────────────────────
  getInitial(name: string): string {
    return name.charAt(0).toUpperCase();
  }

  // ── Count active librarians ───────────────────────────────────
  get activeCount(): number {
    return this.librarians.filter(l => l.status === 'Active').length;
  }
}