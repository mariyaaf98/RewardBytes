import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';

export interface NotificationItem {
  id: string;
  type: string;
  title: string;
  message: string;
  isRead: boolean;
  createdAt: string;
}

@Injectable({ providedIn: 'root' })
export class NotificationService {

  private readonly http = inject(HttpClient);
  private readonly apiUrl = 'http://localhost:7000/notifications';

  getNotifications(): Observable<NotificationItem[]> {
    return this.http.get<NotificationItem[]>(this.apiUrl);
  }

  markAllRead(): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/mark-read`, { ids: [] });
  }

  markRead(ids: string[]): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/mark-read`, { ids });
  }
}
