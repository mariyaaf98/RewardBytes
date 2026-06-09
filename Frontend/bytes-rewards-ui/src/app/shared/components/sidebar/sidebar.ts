import {
  Component,
  Input,
  OnInit
} from '@angular/core';

import { CommonModule } from '@angular/common';

import {
  LucideAngularModule,
  LucideIconData
} from 'lucide-angular';

import {
  RouterLink,
  RouterLinkActive
} from '@angular/router';

import { AuthService } from '../../../core/services/auth';


@Component({
  selector: 'app-sidebar',

  standalone: true,

  imports: [
    CommonModule,
    LucideAngularModule,
    RouterLink,
    RouterLinkActive
  ],

  templateUrl: './sidebar.html',

  styleUrl: './sidebar.css'
})

export class SidebarComponent
  implements OnInit {


  @Input()
  workspaceTitle = '';


  // @Input()
  // menuItems: {
  //   label: string;
  //   icon: LucideIconData;
  //   route: string;
  // }[] = [];

@Input()
menuItems: {
  label: string;
  icon: LucideIconData;
  route?: string;
  children?: {
    label: string;
    route: string;
  }[];
}[] = [];

  expandedMenu = '';

  userName = '';

  userEmail = '';

  initials = '';


  constructor(
    private authService: AuthService
  ) { }


  ngOnInit(): void {

    this.userName =
      this.authService.getUserName();


    this.userEmail =
      this.authService.getUserEmail();


    this.initials =
      this.authService.getUserInitials();

  }

  toggleMenu(label: string): void {

    this.expandedMenu =
      this.expandedMenu === label
        ? ''
        : label;

  }


  logout(): void {

    this.authService.logout();

  }

}