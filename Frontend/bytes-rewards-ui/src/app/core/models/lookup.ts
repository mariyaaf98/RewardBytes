export interface Role {

  id: string;

  name: string;

  description: string;

}


export interface CreateRoleRequest {

  name: string;

  description: string;

}


export interface Department {

  id: string;

  name: string;

}


export interface UserLookup {

  id: string;

  fullName: string;

}