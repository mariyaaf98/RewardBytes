import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';
import { ModalComponent } from '../../../shared/components/modal/modal';
import { ADMIN_MENU } from '../../../core/navigation/admin-menu';
import { UserService } from '../../../core/services/user';
import { User } from '../../../core/models/user.model';
import { Role, Department } from '../../../core/models/lookup';
import { RoleService } from '../../../core/services/role';


@Component({
  selector: 'app-employee-management',

  standalone: true,

  imports: [
    SidebarComponent,
    TopbarComponent,
    ModalComponent,
    FormsModule
  ],

  templateUrl: './employee-management.html',

  styleUrl: './employee-management.css'
})

export class EmployeeManagementComponent
  implements OnInit {

  private readonly userService = inject(UserService);

  private readonly roleService = inject(RoleService);

  isAddEmployeeModalOpen = false;

  adminMenu = ADMIN_MENU;

  // FORM VALUES
  firstName = '';

  lastName = '';

  email = '';

  phoneNumber = '';

  designation = '';

  errorMessage = '';

  showErrorModal = false;

  temporaryPassword = '';

  role = '';

  departmentId = '';

  // REAL EMPLOYEES
  employees: User[] = [];

  selectedStatus = 'all';

  selectedDepartment = 'all';

  searchText = '';

  isEditMode = false;

  selectedUserId = '';


  firstNameError = '';
  emailError = '';
  phoneNumberError = '';
  designationError = '';
  departmentError = '';
  roleError = '';

  // DROPDOWNS
  roles: Role[] = [];

  departments: Department[] = [];

  ngOnInit(): void {

    this.loadUsers();

    this.loadRoles();

    this.loadDepartments();

  }

  loadRoles(): void {

    this.roleService.getRoles()
      .subscribe({

        next: (response) => {

          this.roles = response.filter(
            role =>
              role.name !== 'offline_access' &&
              role.name !== 'uma_authorization'
          );

          console.log('ROLES');
          console.log(this.roles);

        },

        error: (error) => {

          console.error(error);

        }

      });

  }

  loadDepartments(): void {

    this.userService.getDepartments()
      .subscribe({

        next: (response) => {

          this.departments = response;

          console.log(response);

        },

        error: (error) => {

          console.error(error);

        }

      });

  }

  loadUsers(): void {

    this.userService.getUsers()
      .subscribe({

        next: (response) => {

          this.employees = response;

          console.log(response);

        },

        error: (error) => {

          console.error(error);

        }

      });

  }

  createUser(): void {

  if (!this.validateForm()) {

    return;
  }

  this.userService.createUser({

    firstName: this.firstName,

    lastName: this.lastName,

    email: this.email,

    phoneNumber: this.phoneNumber,

    designation: this.designation,

    temporaryPassword: this.temporaryPassword,

    role: this.role,

    departmentId: this.departmentId

  }).subscribe({

    next: () => {

      console.log('USER CREATED');

      this.isAddEmployeeModalOpen = false;

      this.resetForm();

      this.loadUsers();

    },

    error: (error) => {

      this.handleError(error);

    }

  });

}


  deleteUser(id: string): void {

    this.userService.deleteUser(id)
      .subscribe({

        next: () => {

          console.log('DELETE CLICKED');
          console.log('USER ID:', id);

          this.loadUsers();

        },

        error: (error) => {

          this.handleError(error);

        }

      });

  }



  editUser(user: User): void {

    this.isEditMode = true;

    this.selectedUserId = user.id;

    this.firstName = user.firstName;

    this.lastName = user.lastName;

    this.email = user.email;

    this.phoneNumber = user.phoneNumber;

    this.designation = user.designation;

    this.role = user.roleName;

    this.departmentId = user.departmentId;

    this.isAddEmployeeModalOpen = true;

  }

  updateUser(): void {


    this.userService.updateUser(
      this.selectedUserId,
      {

        firstName: this.firstName,

        lastName: this.lastName,

        phoneNumber: this.phoneNumber,

        designation: this.designation,

        role: this.role,

        departmentId: this.departmentId

      }
    )
      .subscribe({


        next: () => {


          console.log('USER UPDATED');


          this.isAddEmployeeModalOpen = false;


          this.resetForm();


          this.loadUsers();


        },


        error: (error) => {

          this.handleError(error);

        }


      });


  }


  updateTemporaryPassword(): void {

    if (!this.firstName.trim()) {

      this.temporaryPassword = '';

      return;
    }

    const formattedName =
      this.firstName.charAt(0).toUpperCase() +
      this.firstName.slice(1).toLowerCase();

    this.temporaryPassword =
      `${formattedName}@Welcome123`;


    console.log(this.temporaryPassword);
  }



  get filteredEmployees(): User[] {

    let filtered = this.employees;


    // SEARCH FILTER
    if (this.searchText.trim() !== '') {

      const search =
        this.searchText.toLowerCase();


      filtered = filtered.filter(employee =>

        employee.firstName
          .toLowerCase()
          .includes(search)

        ||

        employee.lastName
          .toLowerCase()
          .includes(search)

        ||

        employee.email
          .toLowerCase()
          .includes(search)

        ||

        employee.employeeId
          .toLowerCase()
          .includes(search)

      );

    }


    // DEPARTMENT FILTER
    if (this.selectedDepartment !== 'all') {

      filtered = filtered.filter(
        employee =>
          employee.departmentName === this.selectedDepartment
      );

    }


    // STATUS FILTER
    if (this.selectedStatus === 'active') {

      filtered = filtered.filter(
        employee => employee.isActive
      );

    }


    if (this.selectedStatus === 'inactive') {

      filtered = filtered.filter(
        employee => !employee.isActive
      );

    }


    return filtered;

  }

  resetForm(): void {

    this.firstName = '';

    this.lastName = '';

    this.email = '';

    this.phoneNumber = '';

    this.designation = '';

    this.temporaryPassword = '';

    this.role = '';

    this.departmentId = '';

    this.selectedUserId = '';

    this.isEditMode = false;

  }

  private handleError(error: any): void {

    console.error(error);

    if (error.error?.errors) {

      this.errorMessage =
        Object.values(error.error.errors)
          .flat()
          .join('\n');

    }
    else {

      this.errorMessage =
        error.error?.detail ??
        error.error?.message ??
        'Something went wrong';

    }

    this.showErrorModal = true;

  }

  closeErrorModal(): void {

    this.showErrorModal = false;

  }


  private validateForm(): boolean {

    this.firstNameError = '';
    this.emailError = '';
    this.phoneNumberError = '';
    this.designationError = '';
    this.departmentError = '';
    this.roleError = '';

    let isValid = true;

    if (!this.firstName.trim()) {

      this.firstNameError = 'First name is required';

      isValid = false;
    }


    if (!this.email.trim()) {

      this.emailError = 'Email is required';

      isValid = false;
    }
    else if (
      !/^[^\s@]+@[^\s@]+\.[^\s@]+$/.test(this.email)
    ) {

      this.emailError = 'Invalid email format';

      isValid = false;
    }

    if (!this.phoneNumber.trim()) {

      this.phoneNumberError = 'Phone number is required';

      isValid = false;
    }
    else if (
      !/^\d{10}$/.test(this.phoneNumber)
    ) {

      this.phoneNumberError =
        'Phone number must contain exactly 10 digits';

      isValid = false;
    }

    if (!this.designation.trim()) {

      this.designationError = 'Designation is required';

      isValid = false;
    }

    if (!this.departmentId) {

      this.departmentError =
        'Please select a department';

      isValid = false;
    }

    if (!this.role) {

      this.roleError =
        'Please select a role';

      isValid = false;
    }

    return isValid;
  }

}