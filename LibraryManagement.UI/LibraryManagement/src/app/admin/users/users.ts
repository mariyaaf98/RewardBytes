// admin/users/users.ts
// Users page — Add, Edit/Update, Delete users

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-users',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './users.html',
  styleUrl: './users.css'
})
export class UsersComponent {

  // ── Modal control ─────────────────────────────────────────────
  showModal  = false;
  isEditMode = false;    // false = Add mode, true = Edit mode
  editingId  = 0;        // stores the id of the user being edited

  // ── Toast notification ────────────────────────────────────────
  toastMessage = '';
  toastType    = '';

  // ── Form fields (used for both Add and Edit) ──────────────────
  form = {
    name:   '',
    email:  '',
    phone:  '',
    status: 'Active'
  };

  // ── Users list ────────────────────────────────────────────────
  users = [
    { id: 1, name: 'Aisha Rahman', email: 'aisha@email.com',  phone: '012-3456789', status: 'Active'   },
    { id: 2, name: 'Ben Tan',      email: 'ben@email.com',    phone: '011-9876543', status: 'Active'   },
    { id: 3, name: 'Chitra Nair',  email: 'chitra@email.com', phone: '019-1122334', status: 'Inactive' },
    { id: 4, name: 'David Lim',    email: 'david@email.com',  phone: '017-5566778', status: 'Active'   },
    { id: 5, name: 'Evan Raj',     email: 'evan@email.com',   phone: '016-9988776', status: 'Active'   },
  ];

  // ── Open modal for ADD ────────────────────────────────────────
  openAddModal() {
    this.isEditMode = false;          // set to ADD mode
    this.editingId  = 0;
    this.form = { name: '', email: '', phone: '', status: 'Active' };
    this.showModal = true;
  }

  // ── Open modal for EDIT ───────────────────────────────────────
  // fill the form with the selected user's data
  openEditModal(user: any) {
    this.isEditMode = true;           // set to EDIT mode
    this.editingId  = user.id;        // remember which user we are editing

    // fill the form with existing data
    this.form = {
      name:   user.name,
      email:  user.email,
      phone:  user.phone,
      status: user.status
    };

    this.showModal = true;
  }

  // ── Close modal ───────────────────────────────────────────────
  closeModal() {
    this.showModal = false;
  }

  // ── Save — decides Add or Update based on isEditMode ─────────
  saveUser() {
    // Validate required fields
    if (!this.form.name || !this.form.email) {
      this.showToast('Please fill in Name and Email!', 'error');
      return;
    }

    if (this.isEditMode) {
      // ── UPDATE existing user ──
      // find the user in the array and update their data
      const index = this.users.findIndex(u => u.id === this.editingId);
      if (index !== -1) {
        this.users[index] = {
          id:     this.editingId,
          name:   this.form.name,
          email:  this.form.email,
          phone:  this.form.phone,
          status: this.form.status
        };
      }
      this.showToast('User updated successfully!', 'success');

    } else {
      // ── ADD new user ──
      const newUser = {
        id:     Date.now(),
        name:   this.form.name,
        email:  this.form.email,
        phone:  this.form.phone,
        status: this.form.status
      };
      this.users.push(newUser);
      this.showToast('User created successfully!', 'success');
    }

    this.showModal = false;
  }

  // ── Delete user ───────────────────────────────────────────────
  deleteUser(id: number) {
    this.users = this.users.filter(u => u.id !== id);
    this.showToast('User deleted.', 'error');
  }

  // ── Toast helper ──────────────────────────────────────────────
  showToast(message: string, type: string) {
    this.toastMessage = message;
    this.toastType    = type;
    setTimeout(() => { this.toastMessage = ''; }, 2500);
  }

  // ── Avatar initial ────────────────────────────────────────────
  getInitial(name: string): string {
    return name.charAt(0).toUpperCase();
  }

  // ── Count active users ────────────────────────────────────────
  get activeCount(): number {
    return this.users.filter(u => u.status === 'Active').length;
  }
}