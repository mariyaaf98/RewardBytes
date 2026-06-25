import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { HttpClient } from '@angular/common/http';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

interface AdminRedemption {
  redemptionId:  string;
  userId:        string;
  userName:      string;   // employee who requested
  productName:   string;
  redeemedBytes: number;
  status:        string;
  redeemedAt:    string;
}

@Component({
  selector: 'app-redemption-management',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, SidebarComponent, TopbarComponent],
  templateUrl: './redemption-management.html',
  styleUrl: './redemption-management.css'
})
export class RedemptionManagementComponent implements OnInit {

  private readonly http = inject(HttpClient);

  readonly adminMenu = ADMIN_MENU;

  readonly redemptions  = signal<AdminRedemption[]>([]);
  readonly isLoading    = signal(true);
  readonly error        = signal('');
  readonly searchText   = signal('');
  readonly statusFilter = signal('all');

  updatingId    = '';
  updateError   = '';
  updateSuccess = '';

  readonly filtered = computed(() => {
    let list = this.redemptions();
    const q  = this.searchText().toLowerCase().trim();
    if (q) list = list.filter(r =>
      r.productName.toLowerCase().includes(q) ||
      r.userName.toLowerCase().includes(q)
    );
    const s = this.statusFilter();
    if (s !== 'all') list = list.filter(r => r.status === s);
    return list;
  });

  readonly totalCount     = computed(() => this.redemptions().length);
  readonly pendingCount   = computed(() => this.redemptions().filter(r => r.status === 'Pending').length);
  readonly deliveredCount = computed(() => this.redemptions().filter(r => r.status === 'Delivered').length);
  readonly rejectedCount  = computed(() => this.redemptions().filter(r => r.status === 'Rejected').length);

  ngOnInit(): void { this.load(); }

  load(): void {
    this.isLoading.set(true);
    this.error.set('');
    this.http.get<AdminRedemption[]>('http://localhost:7000/redemptions').subscribe({
      next:  d => { this.redemptions.set(d); this.isLoading.set(false); },
      error: e => {
        this.error.set(e.error?.detail ?? 'Failed to load redemptions.');
        this.isLoading.set(false);
      }
    });
  }

  updateStatus(redemptionId: string, status: string): void {
    this.updatingId  = redemptionId;
    this.updateError = '';

    this.http.put<string>('http://localhost:7000/redemptions/status', { redemptionId, status }).subscribe({
      next: () => {
        this.updatingId    = '';
        this.updateSuccess = `Status updated to "${status}".${status === 'Rejected' ? ' Bytes have been refunded to the employee.' : ''}`;
        this.load();
        setTimeout(() => this.updateSuccess = '', 4000);
      },
      error: e => {
        this.updatingId  = '';
        this.updateError = e.error?.detail ?? 'Failed to update status.';
        setTimeout(() => this.updateError = '', 4000);
      }
    });
  }

  onSearch(v: string):  void { this.searchText.set(v); }
  setStatus(s: string): void { this.statusFilter.set(s); }

  statusClass(status: string): string {
    switch (status) {
      case 'Pending':   return 'bg-amber-50 text-amber-600 border border-amber-200';
      case 'Approved':  return 'bg-blue-50 text-blue-600 border border-blue-200';
      case 'Delivered': return 'bg-emerald-50 text-emerald-600 border border-emerald-200';
      case 'Rejected':  return 'bg-red-50 text-red-500 border border-red-200';
      default:          return 'bg-slate-100 text-slate-500';
    }
  }

  nextStatuses(current: string): string[] {
    switch (current) {
      case 'Pending':   return ['Approved', 'Rejected'];
      case 'Approved':  return ['Delivered', 'Rejected'];
      default:          return [];
    }
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
