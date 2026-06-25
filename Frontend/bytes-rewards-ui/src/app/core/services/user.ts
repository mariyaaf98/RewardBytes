import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CreateUserRequest, CurrentUser, UpdateUserRequest, User } from '../models/user.model';
import { Department, UserLookup } from '../models/lookup';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  private readonly http = inject(HttpClient);

  private readonly apiUrl =
    'http://localhost:7000/users';

  getUsers(): Observable<User[]> {

    return this.http.get<User[]>(
      this.apiUrl
    );

  }

  getDepartments(): Observable<Department[]> {

    return this.http.get<Department[]>(
      'http://localhost:7000/departments'
    );

  }


  createUser(
    request: CreateUserRequest
  ): Observable<string> {

    return this.http.post<string>(
      this.apiUrl,
      request
    );

  }

  deleteUser(id: string): Observable<boolean> {

    return this.http.delete<boolean>(
      `${this.apiUrl}/${id}`
    );

  }

  updateUser(
    id: string,
    request: UpdateUserRequest
  ) {

    return this.http.put(
      `${this.apiUrl}/${id}`,
      request
    );

  }

  toggleUserStatus(id: string): Observable<boolean> {

    return this.http.patch<boolean>(
      `${this.apiUrl}/${id}/toggle-status`,
      {}
    );

  }

  getCurrentUser(): Observable<CurrentUser> {
    return this.http.get<CurrentUser>(
      `${this.apiUrl}/me`
    );
  }

  updateCurrentUser(request: { firstName: string; lastName: string; phoneNumber: string }): Observable<boolean> {
    return this.http.put<boolean>(`${this.apiUrl}/me`, request);
  }

  updateProfileImage(profileImageUrl: string): Observable<boolean> {
    return this.http.put<boolean>(
      `${this.apiUrl}/me/profile-image`,
      { profileImageUrl }
    );
  }

  changePassword(currentPassword: string, newPassword: string): Observable<boolean> {
    return this.http.put<boolean>(
      `${this.apiUrl}/me/password`,
      { currentPassword, newPassword }
    );
  }

  getUserLookup(): Observable<UserLookup[]> {
    return this.http.get<UserLookup[]>(`${this.apiUrl}/lookup`);
  }

}
