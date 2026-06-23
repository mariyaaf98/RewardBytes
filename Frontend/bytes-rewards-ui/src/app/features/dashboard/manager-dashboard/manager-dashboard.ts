import { Component, inject, OnInit, signal, computed } from '@angular/core';
import { CommonModule, DatePipe } from '@angular/common';
import { Router } from '@angular/router';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { MANAGER_MENU } from '../../../core/navigation/manager-menu';

import { RewardService, RewardResponse } from '../../../core/services/reward';
import { AppreciationService } from '../../../core/services/appreciation';
import { LeaderboardService, LeaderboardEntry } from '../../../core/services/leaderboard';
import { AuthService } from '../../../core/services/auth';
import { Appreciation } from '../../../core/models/appreciation';

interface EmployeeRow {
  name:             string;
  initials:         string;
  department:       string;
  received:         number;   // appreciations received
  sent:             number;   // appreciations sent
  rank:             number | null;
  score:            number;   // 0-100 relative to top
  // Recommendation logic
  recentlyAwarded:  boolean;  // awarded within last 30 days
  lastAwardedDate:  Date | null;
  recommend:        boolean;  // true = should be awarded now
  recommendReason:  string;   // why
}

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [CommonModule, SidebarComponent, TopbarComponent],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.css'
})
export class ManagerDashboardComponent implements OnInit {

  private readonly rewardService       = inject(RewardService);
  private readonly appreciationService = inject(AppreciationService);
  private readonly leaderboardService  = inject(LeaderboardService);
  private readonly authService         = inject(AuthService);
  private readonly router              = inject(Router);

  readonly managerMenu = MANAGER_MENU;
  readonly managerName = signal('');

  readonly rewards       = signal<RewardResponse[]>([]);
  readonly appreciations = signal<Appreciation[]>([]);
  readonly leaderboard   = signal<LeaderboardEntry[]>([]);
  readonly isLoading     = signal(true);

  // ── Employee appreciation insights table ──────────────────────
  readonly employeeRows = computed<EmployeeRow[]>(() => {
    const apprs    = this.appreciations();
    const lb       = this.leaderboard();
    const allRewards = this.rewards();

    const now     = new Date();
    const days30  = 30 * 24 * 60 * 60 * 1000;

    // Build per-person maps
    const received = new Map<string, number>();
    const sent     = new Map<string, number>();

    apprs.forEach(a => {
      received.set(a.toUserName,   (received.get(a.toUserName)   ?? 0) + 1);
      sent.set(a.fromUserName, (sent.get(a.fromUserName) ?? 0) + 1);
    });

    // Find the most recent award date per recipient
    const lastAward = new Map<string, Date>();
    allRewards.forEach(r => {
      const d = new Date(r.createdAt);
      const existing = lastAward.get(r.toUserName);
      if (!existing || d > existing) lastAward.set(r.toUserName, d);
    });

    const names = new Set([...received.keys(), ...sent.keys()]);
    const maxReceived = Math.max(...[...received.values()], 1);

    const rows: EmployeeRow[] = [];
    names.forEach(name => {
      const r   = received.get(name) ?? 0;
      const s   = sent.get(name) ?? 0;
      if (r === 0 && s === 0) return;

      const entry       = lb.find(e => e.employeeName === name);
      const lastDate    = lastAward.get(name) ?? null;
      const recentlyAw  = lastDate !== null && (now.getTime() - lastDate.getTime()) < days30;

      // Recommendation criteria:
      // 1. Has ≥ 2 peer appreciations (strong signal)
      // 2. Not awarded in the last 30 days
      const recommend = r >= 2 && !recentlyAw;

      // Human-readable reason
      let reason = '';
      if (r >= 2 && !recentlyAw && lastDate === null)
        reason = `${r} peer appreciations — never awarded`;
      else if (r >= 2 && !recentlyAw)
        reason = `${r} appreciations · last award > 30 days ago`;
      else if (recentlyAw)
        reason = `Awarded recently — good to go`;
      else if (r < 2)
        reason = `${r} appreciation${r === 1 ? '' : 's'} — below threshold`;

      rows.push({
        name,
        initials:        this.getInitials(name),
        department:      'Team',
        received:        r,
        sent:            s,
        rank:            entry?.rank ?? null,
        score:           Math.round((r / maxReceived) * 100),
        recentlyAwarded: recentlyAw,
        lastAwardedDate: lastDate,
        recommend,
        recommendReason: reason
      });
    });

    // Sort: recommend first, then by received count
    return rows.sort((a, b) => {
      if (a.recommend !== b.recommend) return a.recommend ? -1 : 1;
      return b.received - a.received;
    });
  });

  // Top recognizers (by sent count) for the bar chart
  readonly topRecognizers = computed(() => {
    const apprs = this.appreciations();
    const sent  = new Map<string, number>();
    apprs.forEach(a => sent.set(a.fromUserName, (sent.get(a.fromUserName) ?? 0) + 1));
    const max = Math.max(...[...sent.values()], 1);
    return [...sent.entries()]
      .map(([name, count]) => ({ name, count, pct: Math.round((count / max) * 100) }))
      .sort((a, b) => b.count - a.count)
      .slice(0, 6);
  });

  readonly totalAwards        = computed(() => this.rewards().length);
  readonly totalBytesGiven    = computed(() => this.rewards().reduce((s, r) => s + (r.bytes ?? 0), 0));
  readonly totalAppreciations = computed(() => this.appreciations().length);
  readonly pendingCount = computed(() => this.employeeRows().filter(e => e.recommend).length);

  ngOnInit(): void {
    this.managerName.set(this.authService.getUserName());
    this.loadData();
  }

  private loadData(): void {
    let loaded = 0;
    const done = () => { if (++loaded >= 3) this.isLoading.set(false); };

    this.rewardService.getRewards().subscribe({
      next: d => { this.rewards.set(d); done(); }, error: () => done()
    });
    this.appreciationService.getAppreciations().subscribe({
      next: d => { this.appreciations.set(d); done(); }, error: () => done()
    });
    this.leaderboardService.getLeaderboard().subscribe({
      next: d => { this.leaderboard.set(d); done(); }, error: () => done()
    });
  }

  goToRecognize():   void { this.router.navigate(['/manager/recognize']); }
  goToLeaderboard(): void { this.router.navigate(['/leaderboard']); }

  getInitials(name: string): string {
    return name.split(' ').map(n => n[0]).join('').toUpperCase().slice(0, 2);
  }

  avatarColor(index: number): string {
    const c = ['#1e3a5f','#065f46','#1e40af','#7c3aed','#b45309','#be185d','#0f766e','#c2410c'];
    return c[index % c.length];
  }
}
