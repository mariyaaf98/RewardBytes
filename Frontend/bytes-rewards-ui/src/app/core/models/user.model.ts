export interface User {

  id: string;

  employeeId: string;

  firstName: string;

  lastName: string;

  email: string;

  phoneNumber: string;

  designation: string;

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

  designation: string;

  temporaryPassword: string;

  role: string;

  departmentId: string;

}

export interface UpdateUserRequest {

  firstName: string;

  lastName: string;

  phoneNumber: string;

  designation: string;

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

  designation: string;

  profileImageUrl: string;

  isActive: boolean;

  departmentId: string;

  departmentName: string;

}