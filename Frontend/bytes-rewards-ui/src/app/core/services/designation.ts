import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Designation } from '../models/lookup';

export interface CreateDesignationRequest {
  name: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class DesignationService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:7000/designations';

  getDesignations(): Observable<Designation[]> {
    return this.http.get<Designation[]>(this.apiUrl);
  }

  createDesignation(request: CreateDesignationRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  updateDesignation(id: string, request: CreateDesignationRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteDesignation(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
