import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Department } from '../models/lookup';

export interface CreateDepartmentRequest {
  name: string;
  description: string;
}

@Injectable({
  providedIn: 'root'
})
export class DepartmentService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl = 'http://localhost:7000/departments';

  getDepartments(): Observable<Department[]> {
    return this.http.get<Department[]>(this.apiUrl);
  }

  createDepartment(request: CreateDepartmentRequest): Observable<string> {
    return this.http.post<string>(this.apiUrl, request);
  }

  // Ready for when backend adds these endpoints
  updateDepartment(id: string, request: CreateDepartmentRequest): Observable<void> {
    return this.http.put<void>(`${this.apiUrl}/${id}`, request);
  }

  deleteDepartment(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

}
