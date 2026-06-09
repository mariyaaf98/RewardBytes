import {
  Component,
  OnInit
} from '@angular/core';

import { LucideAngularModule } from 'lucide-angular';

import {
  Search,
  Bell,
  ChevronDown
} from 'lucide-angular';

import { AuthService } from '../../../core/services/auth';


@Component({
  selector: 'app-topbar',

  standalone: true,

  imports: [
    LucideAngularModule
  ],

  templateUrl: './topbar.html',

  styleUrl: './topbar.css'
})

export class TopbarComponent
implements OnInit {


  readonly Search = Search;

  readonly Bell = Bell;

  readonly ChevronDown = ChevronDown;


  userName = '';

  userEmail = '';

  initials = '';


  constructor(
    private authService: AuthService
  ) {}


  ngOnInit(): void {

    this.userName =
      this.authService.getUserName();


    this.userEmail =
      this.authService.getUserEmail();


    this.initials =
      this.authService.getUserInitials();

  }

}