import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule, DatePipe } from '@angular/common';
import { SidebarComponent } from '../../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../../core/navigation/employee-menu';
import { AppreciationService } from '../../../../core/services/appreciation';
import { AuthService } from '../../../../core/services/auth';
import { Appreciation } from '../../../../core/models/appreciation';

interface HistoryItem {
  id: string;
  type: 'Received' | 'Sent';
  counterpartName: string;
  initials: string;
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
  private readonly authService = inject(AuthService);

  employeeMenu = EMPLOYEE_MENU;

  searchText = '';
  activeFilter: 'all' | 'received' | 'sent' = 'all';
  isLoading = true;

  currentUserName = '';
  history: HistoryItem[] = [];

  ngOnInit(): void {
    this.currentUserName = this.authService.getUserName();
    this.loadAppreciations();
  }

  loadAppreciations(): void {
    this.isLoading = true;
    this.appreciationService.getAppreciations().subscribe({
      next: (appreciations) => {
        this.history = appreciations.map(a => this.mapToHistoryItem(a));
        this.isLoading = false;
      },
      error: (err) => {
        console.error(err);
        this.isLoading = false;
      }
    });
  }

  private mapToHistoryItem(a: Appreciation): HistoryItem {
    const isSent = a.fromUserName === this.currentUserName;
    const counterpart = isSent ? a.toUserName : a.fromUserName;
    return {
      id: a.id,
      type: isSent ? 'Sent' : 'Received',
      counterpartName: counterpart,
      initials: this.getInitials(counterpart),
      message: a.message,
      createdAt: a.createdAt,
      likesCount: a.likesCount ?? 0
    };
  }

  get totalActivity(): number { return this.history.length; }
  get totalReceived(): number { return this.history.filter(h => h.type === 'Received').length; }
  get totalSent(): number { return this.history.filter(h => h.type === 'Sent').length; }

  setFilter(f: 'all' | 'received' | 'sent'): void {
    this.activeFilter = f;
  }

  get filteredHistory(): HistoryItem[] {
    let items = this.history;

    if (this.activeFilter === 'received') items = items.filter(h => h.type === 'Received');
    else if (this.activeFilter === 'sent') items = items.filter(h => h.type === 'Sent');

    if (this.searchText.trim()) {
      const q = this.searchText.toLowerCase();
      items = items.filter(h =>
        h.counterpartName.toLowerCase().includes(q) ||
        h.message.toLowerCase().includes(q)
      );
    }

    return items;
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
