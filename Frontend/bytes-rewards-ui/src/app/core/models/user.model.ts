export interface User {

  id: string;

  employeeId: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber: string;

  designationId: string;

  designationName: string;

  totalBytes: number;

  isActive: boolean;

  role: string;

  departmentId: string;

  roleName: string;

  departmentName: string;

}

export interface CreateUserRequest {

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber: string;

  designationId: string;

  temporaryPassword: string;

  role: string;

  departmentId: string;

}

export interface UpdateUserRequest {

  firstName: string;

  lastName: string;

  phoneNumber: string;

  designationId: string;

  role: string;

  departmentId: string;

}

export interface CurrentUser {

  id: string;

  employeeId: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber: string;

  designationId: string;

  designationName: string;

  profileImageUrl: string;

  isActive: boolean;

  departmentId: string;

  departmentName: string;

}