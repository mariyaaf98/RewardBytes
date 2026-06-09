import { Component } from '@angular/core';

import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';

import { MANAGER_MENU } from '../../../core/navigation/manager-menu';

@Component({
  selector: 'app-manager-dashboard',
  standalone: true,
  imports: [
    SidebarComponent,
    TopbarComponent
  ],
  templateUrl: './manager-dashboard.html',
  styleUrl: './manager-dashboard.css'
})
export class ManagerDashboardComponent {

  managerMenu = MANAGER_MENU;

}