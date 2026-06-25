import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';
import {
  RewardService,
  EmployeeRewardSummary,
  EmployeeRewardStatusResponse
} from '../../../core/services/reward';

type Tab = 'notRewarded' | 'rewarded';

@Component({
  selector: 'app-team-rewards',
  standalone: true,
  imports: [CommonModule, DatePipe, FormsModule, SidebarComponent, TopbarComponent],
  templateUrl: './team-rewards.html',
  styleUrl: './team-rewards.css'
})
export class TeamRewardsComponent implements OnInit {

  private readonly rewardService = inject(RewardService);

  readonly managerMenu = MANAGER_MENU;
  readonly activeTab   = signal<Tab>('notRewarded');
  readonly isLoading   = signal(true);
  readonly searchText  = signal('');

  readonly data = signal<EmployeeRewardStatusResponse>({
    rewarded: [],
    notRewarded: []
  });

  readonly currentList = computed<EmployeeRewardSummary[]>(() => {
    const q    = this.searchText().toLowerCase().trim();
    const list = this.activeTab() === 'notRewarded'
      ? this.data().notRewarded
      : this.data().rewarded;

    if (!q) return list;
    return list.filter(e =>
      e.fullName.toLowerCase().includes(q) ||
      e.departmentName.toLowerCase().includes(q) ||
      e.designationName.toLowerCase().includes(q)
    );
  });

  ngOnInit(): void {
    this.rewardService.getEmployeeRewardStatus().subscribe({
      next: res => {
        this.data.set(res);
        this.isLoading.set(false);
      },
      error: () => this.isLoading.set(false)
    });
  }

  setTab(tab: Tab): void {
    this.activeTab.set(tab);
    this.searchText.set('');
  }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }
}
