import {
  inject,
  Injectable
} from '@angular/core';

import {
  HttpClient
} from '@angular/common/http';

import {
  Observable
} from 'rxjs';

import {
  Role,
  CreateRoleRequest
} from '../models/lookup';


@Injectable({
  providedIn: 'root'
})
export class RoleService {


  private readonly http =
    inject(HttpClient);


  private readonly apiUrl =
    'http://localhost:7000/roles';



  getRoles(): Observable<Role[]> {

    return this.http.get<Role[]>(
      this.apiUrl
    );

  }



  createRole(
    request: CreateRoleRequest
  ): Observable<boolean> {

    return this.http.post<boolean>(
      this.apiUrl,
      request
    );

  }


}