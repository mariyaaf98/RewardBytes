import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface CreateRewardRequest {
  toUserId: string;
  rewardCategoryId: string;
  reason: string;
}

export interface RewardResponse {
  id: string;
  fromUserName: string;
  toUserName: string;
  rewardCategoryName: string;
  bytes: number;
  reason: string;
  createdAt: string;
}

export interface EmployeeRewardSummary {
  id: string;
  fullName: string;
  departmentName: string;
  designationName: string;
  lastRewardedAt?: string;
  lastRewardCategoryName: string;
  lastRewardBytes: number;
  totalRewards: number;
}

export interface EmployeeRewardStatusResponse {
  rewarded: EmployeeRewardSummary[];
  notRewarded: EmployeeRewardSummary[];
}

export interface RewardHistoryItem {
  rewardId: string;
  rewardCategoryName: string;
  bytes: number;
  awardedBy: string;
  reason: string;
  awardedAt: string;
}

@Injectable({
  providedIn: 'root'
})
export class RewardService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:7000/rewards';

  getRewards(): Observable<RewardResponse[]> {
    return this.http.get<RewardResponse[]>(this.apiUrl);
  }

  createReward(request: CreateRewardRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  getRewardHistory(userId: string): Observable<RewardHistoryItem[]> {
    return this.http.get<RewardHistoryItem[]>(`${this.apiUrl}/history/${userId}`);
  }

  getEmployeeRewardStatus(): Observable<EmployeeRewardStatusResponse> {
    return this.http.get<EmployeeRewardStatusResponse>(
      `${this.apiUrl}/employee-status`
    );
  }
}
