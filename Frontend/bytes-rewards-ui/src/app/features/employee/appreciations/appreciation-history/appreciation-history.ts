import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { SidebarComponent } from '../../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../../core/navigation/employee-menu';
import { AppreciationService } from '../../../../core/services/appreciation';
import { UserService } from '../../../../core/services/user';
import { Appreciation } from '../../../../core/models/appreciation';

interface HistoryItem {
  id: string;
  type: 'Received' | 'Sent';
  fromName: string;       // who sent it
  toName: string;         // who received it
  fromInitials: string;
  toInitials: string;
  message: string;
  createdAt: string;
  likesCount: number;
}

@Component({
  selector: 'app-appreciation-history',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent, FormsModule, DatePipe],
  templateUrl: './appreciation-history.html',
  styleUrl: './appreciation-history.css'
})
export class AppreciationHistoryComponent implements OnInit {

  private readonly appreciationService = inject(AppreciationService);
  private readonly userService         = inject(UserService);

  employeeMenu = EMPLOYEE_MENU;

  searchText   = '';
  activeFilter: 'all' | 'received' | 'sent' = 'all';
  isLoading    = true;

  // Use DB user id for reliable comparison — not Keycloak name string
  private currentUserId = '';
  history: HistoryItem[] = [];
  allAppreciations: Appreciation[] = [];

  ngOnInit(): void {
    // Get current user's DB id first, then load appreciations
    this.userService.getCurrentUser().subscribe({
      next: (user) => {
        this.currentUserId = user.id;
        this.loadAppreciations();
      },
      error: () => {
        // Fallback: load anyway, type detection will be best-effort
        this.loadAppreciations();
      }
    });
  }

  loadAppreciations(): void {
    this.isLoading = true;
    this.appreciationService.getAppreciations().subscribe({
      next: (appreciations) => {
        this.allAppreciations = appreciations;
        this.history = appreciations.map(a => this.mapToHistoryItem(a));
        this.isLoading = false;
      },
      error: () => { this.isLoading = false; }
    });
  }

  private mapToHistoryItem(a: Appreciation): HistoryItem {
    // Compare by userId (fromUserId on the model) — reliable, not string-matched name
    const isSent = a.fromUserId === this.currentUserId;

    return {
      id:           a.id,
      type:         isSent ? 'Sent' : 'Received',
      fromName:     a.fromUserName,
      toName:       a.toUserName,
      fromInitials: this.getInitials(a.fromUserName),
      toInitials:   this.getInitials(a.toUserName),
      message:      a.message,
      createdAt:    a.createdAt,
      likesCount:   a.likesCount ?? 0
    };
  }

  get totalActivity(): number { return this.history.length; }
  get totalReceived(): number { return this.history.filter(h => h.type === 'Received').length; }
  get totalSent():     number { return this.history.filter(h => h.type === 'Sent').length; }

  setFilter(f: 'all' | 'received' | 'sent'): void { this.activeFilter = f; }

  get filteredHistory(): HistoryItem[] {
    let items = this.history;
    if (this.activeFilter === 'received') items = items.filter(h => h.type === 'Received');
    else if (this.activeFilter === 'sent')  items = items.filter(h => h.type === 'Sent');

    if (this.searchText.trim()) {
      const q = this.searchText.toLowerCase();
      items = items.filter(h =>
        h.fromName.toLowerCase().includes(q) ||
        h.toName.toLowerCase().includes(q)   ||
        h.message.toLowerCase().includes(q)
      );
    }
    return items;
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
