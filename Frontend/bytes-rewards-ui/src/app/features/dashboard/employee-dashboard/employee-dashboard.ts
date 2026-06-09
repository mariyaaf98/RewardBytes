import { Component } from '@angular/core';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';

import { EMPLOYEE_MENU } from '../../../core/navigation/employee-menu';

@Component({
  selector: 'app-employee-dashboard',
  standalone: true,
  imports: [
    SidebarComponent,
    TopbarComponent
  ],
  templateUrl: './employee-dashboard.html',
  styleUrl: './employee-dashboard.css'
})
export class EmployeeDashboardComponent {

  employeeMenu = EMPLOYEE_MENU;

}