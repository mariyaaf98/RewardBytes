import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface LeaderboardEntry {
  rank: number;
  userId: string;
  employeeName: string;
  totalEarnedBytes: number;
}

@Injectable({
  providedIn: 'root'
})
export class LeaderboardService {

  private readonly http = inject(HttpClient);

  getLeaderboard(): Observable<LeaderboardEntry[]> {
    return this.http.get<LeaderboardEntry[]>(
      'http://localhost:7000/leaderboard'
    );
  }
}
