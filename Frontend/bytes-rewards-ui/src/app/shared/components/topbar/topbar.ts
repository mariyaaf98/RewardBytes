import { Component, OnInit, HostListener, ElementRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LucideAngularModule, Search, Bell } from 'lucide-angular';
import { Router } from '@angular/router';
import { AuthService } from '../../../core/services/auth';
import { UserService } from '../../../core/services/user';

@Component({
  selector: 'app-topbar',
  standalone: true,
  imports: [CommonModule, LucideAngularModule],
  templateUrl: './topbar.html',
  styleUrl: './topbar.css'
})
export class TopbarComponent implements OnInit {

  readonly Search = Search;
  readonly Bell   = Bell;

  userName        = '';
  userEmail       = '';
  userDesignation = '';
  initials        = '';
  role            = '';

  dropdownOpen = false;

  constructor(
    private authService: AuthService,
    private userService: UserService,
    private router: Router,
    private elRef: ElementRef
  ) {}

  ngOnInit(): void {
    this.userName  = this.authService.getUserName();
    this.userEmail = this.authService.getUserEmail();
    this.initials  = this.authService.getUserInitials();
    this.role      = this.authService.currentRole();

    this.userService.getCurrentUser().subscribe({
      next: (user) => { this.userDesignation = user.designation; },
      error: () => {}
    });
  }

  toggleDropdown(): void {
    this.dropdownOpen = !this.dropdownOpen;
  }

  // close dropdown when clicking outside
  @HostListener('document:click', ['$event'])
  onDocumentClick(event: MouseEvent): void {
    if (!this.elRef.nativeElement.contains(event.target)) {
      this.dropdownOpen = false;
    }
  }

  navigate(path: string): void {
    this.dropdownOpen = false;
    this.router.navigate([path]);
  }

  signOut(): void {
    this.dropdownOpen = false;
    this.authService.logout();
  }
}
