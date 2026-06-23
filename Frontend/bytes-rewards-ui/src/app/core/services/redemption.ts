import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RedemptionHistory } from '../models/redemption';

@Injectable({ providedIn: 'root' })
export class RedemptionService {

  private readonly http = inject(HttpClient);
  private readonly base = 'http://localhost:7000';

  redeemReward(userId: string, rewardItemId: string): Observable<string> {
    return this.http.post<string>(`${this.base}/redemptions`, { userId, rewardItemId });
  }

  getRedemptionHistory(userId: string): Observable<RedemptionHistory[]> {
    return this.http.get<RedemptionHistory[]>(`${this.base}/redemptions/history/${userId}`);
  }
}
