import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { TopbarComponent } from '../../../../shared/components/topbar/topbar';
import { SidebarComponent } from '../../../../shared/components/sidebar/sidebar';
import { EMPLOYEE_MENU } from '../../../../core/navigation/employee-menu';
import { UserService } from '../../../../core/services/user';
import { AppreciationService } from '../../../../core/services/appreciation';
import { UserLookup } from '../../../../core/models/lookup';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-create-appreciation',
  standalone: true,
  imports: [
    FormsModule,
    SidebarComponent,
    TopbarComponent,
    DatePipe
  ],
  templateUrl: './create-appreciation.html',
  styleUrl: './create-appreciation.css',
})

export class CreateAppreciationComponent implements OnInit {


  private readonly userService =
    inject(UserService);

  private readonly appreciationService =
    inject(AppreciationService);

  ngOnInit(): void {
    this.loadUsers();

  }



  employeeMenu = EMPLOYEE_MENU;

  selectedUserId = '';

  message = '';

  currentUserId = '';

  showSuccess = false;

  searchText = '';

  errorMessage = '';

  showErrorModal = false;

  today = new Date();



  users: UserLookup[] = [];

  filteredUsers: UserLookup[] = [];

  loadUsers(): void {

    this.userService.getUserLookup()
      .subscribe({

        next: users => {

          console.log(users);

          this.users = users;

          this.filteredUsers = users;

        },

        error: error => {

          this.errorMessage =
            error.error.detail;

          this.showErrorModal = true;

        }

      });

  }

  filterUsers(): void {

    const search =
      this.searchText.toLowerCase();

    this.filteredUsers =
      this.users.filter(user =>
        user.id !== this.currentUserId &&
        user.fullName
          .toLowerCase()
          .includes(search)
      );

  }

  selectUser(user: UserLookup): void {

    this.selectedUserId = user.id;

    this.searchText = user.fullName;

    this.filteredUsers = [];

  }

  submit(): void {

    if (
      !this.selectedUserId ||
      !this.message.trim()
    ) {

      this.errorMessage =
        'Please select an employee and enter a message.';

      this.showErrorModal = true;

      return;

    }

    this.appreciationService
      .createAppreciation({
        toUserId: this.selectedUserId,
        message: this.message
      })
      .subscribe({

        next: () => {

          this.showSuccess = true;

          setTimeout(() => {

            this.showSuccess = false;

            this.selectedUserId = '';

            this.message = '';

            this.searchText = '';

            this.filteredUsers = this.users;

          }, 3000);

        },
        error: error => {

          console.error(error);

          this.errorMessage =
            error.error?.detail ??
            'Something went wrong.';

          this.showErrorModal = true;

        }

      });

  }

  get selectedUserName(): string {

    return this.users.find(
      user => user.id === this.selectedUserId
    )?.fullName ?? 'Select Employee';

  }


  get selectedUserInitials(): string {

    const user =
      this.users.find(
        x => x.id === this.selectedUserId
      );

    if (!user) {
      return '??';
    }

    return user.fullName
      .split(' ')
      .map(x => x[0])
      .join('')
      .toUpperCase();

  }


  closeErrorModal(): void {

    this.showErrorModal = false;

  }

}
