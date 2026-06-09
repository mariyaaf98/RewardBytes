import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Appreciation } from '../models/appreciation';

@Injectable({
  providedIn: 'root'
})
export class AppreciationService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:7000/appreciations';

  getAppreciations(): Observable<Appreciation[]> {

    return this.http.get<Appreciation[]>(
      this.apiUrl
    );

  }

  createAppreciation(
    request: {
      toUserId: string;
      message: string;
    }
  ): Observable<string> {

    return this.http.post<string>(
      this.apiUrl,
      request
    );

  }

}