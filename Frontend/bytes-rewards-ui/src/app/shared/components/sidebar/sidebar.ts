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
import { UserService } from '../../../core/services/user';


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

  expandedMenu     = '';
  userName         = '';
  userEmail        = '';
  initials         = '';
  profileImageUrl  = '';

  constructor(
    private authService: AuthService,
    private userService: UserService
  ) { }

  ngOnInit(): void {
    this.userName  = this.authService.getUserName();
    this.userEmail = this.authService.getUserEmail();
    this.initials  = this.authService.getUserInitials();

    // load profile photo
    this.userService.getCurrentUser().subscribe({
      next: u => { this.profileImageUrl = u.profileImageUrl ?? ''; },
      error: () => {}
    });
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