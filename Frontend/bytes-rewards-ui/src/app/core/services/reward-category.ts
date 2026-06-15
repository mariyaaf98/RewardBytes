import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { RewardCategory, CreateRewardCategoryRequest } from '../models/reward-category';

@Injectable({
  providedIn: 'root'
})
export class RewardCategoryService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:7000/reward-categories';

  getRewardCategories(): Observable<RewardCategory[]> {
    return this.http.get<RewardCategory[]>(this.apiUrl);
  }

  createRewardCategory(request: CreateRewardCategoryRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  updateRewardCategory(id: string, request: CreateRewardCategoryRequest): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/${id}`, request);
  }

  deleteRewardCategory(id: string): Observable<boolean> {
    return this.http.delete<boolean>(`${this.apiUrl}/${id}`);
  }
}
