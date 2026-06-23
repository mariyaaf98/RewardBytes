import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RewardItem } from '../models/reward-item';

export interface CreateRewardItemRequest {
  productCode: string;
  name: string;
  description: string;
  requiredBytes: number;
  isActive: boolean;
  imageUrl: string;
}

@Injectable({ providedIn: 'root' })
export class RewardCatalogService {

  private readonly http = inject(HttpClient);
  private readonly base = 'http://localhost:7000';

  getRewardItems(): Observable<RewardItem[]> {
    return this.http.get<RewardItem[]>(`${this.base}/reward-items`);
  }

  getRewardItemById(id: string): Observable<RewardItem> {
    return this.http.get<RewardItem>(`${this.base}/reward-items/${id}`);
  }

  createRewardItem(request: CreateRewardItemRequest): Observable<string> {
    return this.http.post<string>(`${this.base}/reward-items`, request);
  }

  updateRewardItem(id: string, request: CreateRewardItemRequest): Observable<string> {
    return this.http.put<string>(`${this.base}/reward-items/${id}`, request);
  }

  deleteRewardItem(id: string): Observable<string> {
    return this.http.delete<string>(`${this.base}/reward-items/${id}`);
  }
}
