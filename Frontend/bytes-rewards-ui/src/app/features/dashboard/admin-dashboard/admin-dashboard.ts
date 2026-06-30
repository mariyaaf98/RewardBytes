import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { PieChartComponent, PieSlice } from '../../../shared/components/pie-chart/pie-chart';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

import { UserService } from '../../../core/services/user';
import { RewardService, RewardResponse } from '../../../core/services/reward';
import { AppreciationService } from '../../../core/services/appreciation';
import { LeaderboardService, LeaderboardEntry } from '../../../core/services/leaderboard';
import { RewardCatalogService } from '../../../core/services/reward-catalog';

import { User } from '../../../core/models/user.model';
import { Appreciation } from '../../../core/models/appreciation';
import { HttpClient } from '@angular/common/http';

interface AdminRedemption {
  redemptionId: string;
  productName: string;
  redeemedBytes: number;
  status: string;
  redeemedAt: string;
}

interface DeptStat {
  name:           string;
  employeeCount:  number;
  bytesAwarded:   number;
  appreciations:  number;
  pct:            number;   // relative to max dept bytes
}

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [CommonModule, DatePipe, SidebarComponent, TopbarComponent, PieChartComponent],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboardComponent implements OnInit {

  private readonly userService        = inject(UserService);
  private readonly rewardService      = inject(RewardService);
  private readonly appreciationService = inject(AppreciationService);
  private readonly leaderboardService = inject(LeaderboardService);
  private readonly catalogService     = inject(RewardCatalogService);
  private readonly http               = inject(HttpClient);
  private readonly router             = inject(Router);

  readonly adminMenu = ADMIN_MENU;

  // ── Raw data ──────────────────────────────────────────────────
  readonly users         = signal<User[]>([]);
  readonly rewards       = signal<RewardResponse[]>([]);
  readonly appreciations = signal<Appreciation[]>([]);
  readonly leaderboard   = signal<LeaderboardEntry[]>([]);
  readonly catalogItems  = signal<number>(0);
  readonly redemptions   = signal<AdminRedemption[]>([]);

  readonly isLoading = signal(true);

  // ── Derived stats ────────────────────────────────────────────
  readonly totalEmployees  = computed(() => this.users().length);
  readonly activeEmployees = computed(() => this.users().filter(u => u.isActive).length);
  readonly totalRewards    = computed(() => this.rewards().length);
  readonly totalAppreciations = computed(() => this.appreciations().length);
  readonly pendingRedemptions = computed(() =>
    this.redemptions().filter(r => r.status === 'Pending').length
  );
  readonly totalBytesAwarded = computed(() =>
    this.rewards().reduce((s, r) => s + (r.bytes ?? 0), 0)
  );

  // Recent activity — last 5 rewards, newest first
  readonly recentRewards = computed(() =>
    [...this.rewards()]
      .sort((a, b) => new Date(b.createdAt).getTime() - new Date(a.createdAt).getTime())
      .slice(0, 5)
  );

  // Top 5 from leaderboard
  readonly topEmployees = computed(() => this.leaderboard().slice(0, 5));

  // ── Department analysis ───────────────────────────────────────
  readonly deptStats = computed<DeptStat[]>(() => {
    const users  = this.users();
    const rewards = this.rewards();
    const apprs  = this.appreciations();

    // Build a map: employeeName → departmentName
    const nameToDept = new Map<string, string>();
    users.forEach(u => {
      const fullName = `${u.firstName} ${u.lastName}`;
      nameToDept.set(fullName, u.departmentName || 'Unknown');
    });

    // Count bytes awarded per department (via reward recipient's dept)
    const deptBytes = new Map<string, number>();
    const deptApprs = new Map<string, number>();
    const deptEmps  = new Map<string, Set<string>>();

    rewards.forEach(r => {
      const dept = nameToDept.get(r.toUserName) ?? 'Unknown';
      deptBytes.set(dept, (deptBytes.get(dept) ?? 0) + (r.bytes ?? 0));
      if (!deptEmps.has(dept)) deptEmps.set(dept, new Set());
      deptEmps.get(dept)!.add(r.toUserName);
    });

    apprs.forEach(a => {
      const dept = nameToDept.get(a.toUserName) ?? 'Unknown';
      deptApprs.set(dept, (deptApprs.get(dept) ?? 0) + 1);
    });

    // Also count employees per dept from users list
    const deptEmpCount = new Map<string, number>();
    users.forEach(u => {
      const d = u.departmentName || 'Unknown';
      deptEmpCount.set(d, (deptEmpCount.get(d) ?? 0) + 1);
    });

    const allDepts = new Set([...deptBytes.keys(), ...deptApprs.keys(), ...deptEmpCount.keys()]);
    const maxBytes = Math.max(...[...deptBytes.values()], 1);

    const stats: DeptStat[] = [];
    allDepts.forEach(name => {
      if (name === 'Unknown') return;
      stats.push({
        name,
        employeeCount: deptEmpCount.get(name) ?? 0,
        bytesAwarded:  deptBytes.get(name) ?? 0,
        appreciations: deptApprs.get(name) ?? 0,
        pct:           Math.round(((deptBytes.get(name) ?? 0) / maxBytes) * 100)
      });
    });

    return stats.sort((a, b) => b.bytesAwarded - a.bytesAwarded);
  });

  // ── Pie chart slices for bytes awarded per department ─────────
  readonly DEPT_COLORS = [
    '#10b981', '#3b82f6', '#f59e0b', '#8b5cf6',
    '#ef4444', '#06b6d4', '#f97316', '#ec4899',
    '#84cc16', '#6366f1'
  ];

  readonly bytesPieSlices = computed<PieSlice[]>(() =>
    this.deptStats().map((d, i) => ({
      label: d.name,
      value: d.bytesAwarded,
      color: this.DEPT_COLORS[i % this.DEPT_COLORS.length]
    }))
  );

  readonly apprPieSlices = computed<PieSlice[]>(() =>
    this.deptStats().map((d, i) => ({
      label: d.name,
      value: d.appreciations,
      color: this.DEPT_COLORS[i % this.DEPT_COLORS.length]
    }))
  );
  ngOnInit(): void {
    let loaded = 0;
    const done = () => { if (++loaded >= 5) this.isLoading.set(false); };

    this.userService.getUsers().subscribe({
      next: d => { this.users.set(d); done(); }, error: () => done()
    });

    this.rewardService.getRewards().subscribe({
      next: d => { this.rewards.set(d); done(); }, error: () => done()
    });

    this.appreciationService.getAppreciations().subscribe({
      next: d => { this.appreciations.set(d); done(); }, error: () => done()
    });

    this.leaderboardService.getLeaderboard().subscribe({
      next: d => { this.leaderboard.set(d); done(); }, error: () => done()
    });

    this.catalogService.getRewardItems().subscribe({
      next: d => { this.catalogItems.set(d.length); done(); }, error: () => done()
    });

    // redemptions — best effort
    this.http.get<AdminRedemption[]>('http://localhost:7000/redemptions').subscribe({
      next: d => this.redemptions.set(d), error: () => {}
    });
  }

  // ── Helpers ───────────────────────────────────────────────────
  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  avatarColor(index: number): string {
    const c = ['#1e3a5f','#065f46','#1e40af','#7c3aed','#b45309'];
    return c[index % c.length];
  }

  go(path: string): void { this.router.navigate([path]); }
}
