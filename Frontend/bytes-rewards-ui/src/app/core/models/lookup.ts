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

  description: string;

  isActive: boolean;

}


export interface UserLookup {

  id: string;

  fullName: string;

}