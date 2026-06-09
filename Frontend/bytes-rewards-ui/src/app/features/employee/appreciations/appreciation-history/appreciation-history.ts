import {
  Component,
  inject,
  OnInit
} from '@angular/core';

import { FormsModule } from '@angular/forms';

import { SidebarComponent }
  from '../../../../shared/components/sidebar/sidebar';

import { TopbarComponent }
  from '../../../../shared/components/topbar/topbar';

import { EMPLOYEE_MENU }
  from '../../../../core/navigation/employee-menu';

import { AppreciationService }
  from '../../../../core/services/appreciation';

import { Appreciation }
  from '../../../../core/models/appreciation';
import { DatePipe } from '@angular/common';

@Component({
  selector: 'app-recognition-history',

  standalone: true,

  imports: [
    SidebarComponent,
    TopbarComponent,
    FormsModule,
    DatePipe
  ],

  templateUrl: './appreciation-history.html',

  styleUrl: './appreciation-history.css'
})
export class AppreciationHistoryComponent
  implements OnInit {

  private readonly appreciationService =
    inject(AppreciationService);

  employeeMenu = EMPLOYEE_MENU;

  searchText = '';

  history: Appreciation[] = [];

  ngOnInit(): void {

    this.loadAppreciations();

  }

  loadAppreciations(): void {

    this.appreciationService
      .getAppreciations()
      .subscribe({

        next: (response) => {

          console.log(response);

          this.history = response;

        },

        error: (error) => {

          console.error(error);

        }

      });

  }

  get totalActivity(): number {

    return this.history.length;

  }

  get filteredHistory(): Appreciation[] {

    return this.history.filter(item =>

      item.message
        .toLowerCase()
        .includes(
          this.searchText.toLowerCase()
        )

      ||

      item.fromUserName
        .toLowerCase()
        .includes(
          this.searchText.toLowerCase()
        )

      ||

      item.toUserName
        .toLowerCase()
        .includes(
          this.searchText.toLowerCase()
        )

    );

  }

}