import { Component } from '@angular/core';
import { SidebarComponent } from '../../../shared/components/sidebar/sidebar';
import { TopbarComponent } from '../../../shared/components/topbar/topbar';

import { ADMIN_MENU } from '../../../core/navigation/admin-menu';

@Component({
  selector: 'app-admin-dashboard',
  standalone: true,
  imports: [
    SidebarComponent,
    TopbarComponent
  ],
  templateUrl: './admin-dashboard.html',
  styleUrl: './admin-dashboard.css'
})
export class AdminDashboardComponent {

  adminMenu = ADMIN_MENU;

}