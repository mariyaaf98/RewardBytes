import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';
import { LeaderboardService, LeaderboardEntry } from '../../../core/services/leaderboard';
import { AuthService } from '../../../core/services/auth';

@Component({
  selector: 'app-leaderboard',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent, FormsModule],
  templateUrl: './leaderboard.html',
  styleUrl: './leaderboard.css'
})
export class LeaderboardComponent implements OnInit {

  private readonly leaderboardService = inject(LeaderboardService);
  private readonly authService        = inject(AuthService);

  readonly employeeMenu    = EMPLOYEE_MENU;
  readonly managerMenu     = MANAGER_MENU;
  readonly adminMenu       = ADMIN_MENU;

  get activeMenu() {
    const r = this.authService.currentRole();
    if (r === 'manager') return this.managerMenu;
    if (r === 'admin')   return this.adminMenu;
    return this.employeeMenu;
  }

  get workspaceTitle(): string {
    const r = this.authService.currentRole();
    if (r === 'manager') return 'Manager Workspace';
    if (r === 'admin')   return 'Admin Workspace';
    return 'Employee Workspace';
  }

  readonly entries         = signal<LeaderboardEntry[]>([]);
  readonly isLoading       = signal(true);
  readonly error           = signal('');
  readonly searchText      = signal('');
  readonly currentUserName = signal('');

  // top 3 for podium
  readonly topThree = computed(() => this.entries().slice(0, 3));

  // all entries filtered by search — full list including top 3
  readonly restOfRanking = computed(() => {
    const q = this.searchText().toLowerCase().trim();
    if (!q) return this.entries();
    return this.entries().filter(e => e.employeeName.toLowerCase().includes(q));
  });

  ngOnInit(): void {
    this.currentUserName.set(this.authService.getUserName());
    this.load();
  }

  load(): void {
    this.isLoading.set(true);
    this.error.set('');
    this.leaderboardService.getLeaderboard().subscribe({
      next:  d => { this.entries.set(d); this.isLoading.set(false); },
      error: e => { this.error.set(e.error?.detail ?? 'Failed to load leaderboard.'); this.isLoading.set(false); }
    });
  }

  onSearch(v: string): void { this.searchText.set(v); }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  isCurrentUser(name: string): boolean {
    return name === this.currentUserName();
  }

  // deterministic color per person — cycles through a palette
  avatarColor(index: number): string {
    const colors = [
      '#1e3a5f', // dark navy
      '#065f46', // dark green
      '#1e40af', // dark blue
      '#7c3aed', // purple
      '#b45309', // amber
      '#be185d', // pink
      '#0f766e', // teal
      '#c2410c', // orange
    ];
    return colors[index % colors.length];
  }
}
