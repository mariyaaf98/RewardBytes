// app.ts
// Root component — holds sidebar + header + router-outlet

import { Component } from '@angular/core';
import { RouterOutlet, RouterLink, RouterLinkActive } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [
    RouterOutlet,       // ✅ needed for <router-outlet> in app.html
    RouterLink,         // ✅ needed for [routerLink] in app.html
    RouterLinkActive    // ✅ needed for routerLinkActive in app.html
  ],
  templateUrl: './app.html',
  styleUrl: './app.css'
})
export class AppComponent {   // ✅ must be named AppComponent

  // Sidebar menu items
  navItems = [
    { label: 'Dashboard',  icon: '🏠', link: '/admin/dashboard'  },
    { label: 'Users',      icon: '👤', link: '/admin/users'       },
    { label: 'Librarians', icon: '📚', link: '/admin/librarians'  },
    { label: 'System',     icon: '⚙️', link: '/admin/system'      },
  ];
}