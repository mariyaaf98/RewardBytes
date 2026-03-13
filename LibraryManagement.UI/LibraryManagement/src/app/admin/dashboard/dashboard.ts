// admin/dashboard/dashboard.ts
// Admin Dashboard page

import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent {   // ✅ must be named DashboardComponent

  // Today's date
  today = new Date();

  // 4 Stat Cards
  stats = [
    { label: 'Total Users',      value: 120, icon: '👤', color: '#4fc3f7', bg: '#e8f7fd' },
    { label: 'Total Librarians', value: 8,   icon: '📚', color: '#81c784', bg: '#edf7ed' },
    { label: 'Books Available',  value: 540, icon: '📖', color: '#ffb74d', bg: '#fff8ee' },
    { label: 'Borrowed Books',   value: 76,  icon: '🔖', color: '#ce93d8', bg: '#f8f0fd' },
  ];

  // Recent Users
  recentUsers = [
    { name: 'Aisha Rahman', email: 'aisha@email.com',  status: 'Active'   },
    { name: 'Ben Tan',      email: 'ben@email.com',    status: 'Active'   },
    { name: 'Chitra Nair',  email: 'chitra@email.com', status: 'Inactive' },
    { name: 'David Lim',    email: 'david@email.com',  status: 'Active'   },
  ];

  // Recent Librarians
  recentLibrarians = [
    { name: 'Mr. Hassan', branch: 'Main Branch' },
    { name: 'Ms. Priya',  branch: 'East Wing'   },
    { name: 'Mr. Rajan',  branch: 'West Wing'   },
  ];

  // Activity Feed
  activities = [
    { icon: '👤', text: 'New user Aisha Rahman registered',               time: '2 min ago'   },
    { icon: '📚', text: 'Librarian Mr. Hassan added a new book',          time: '15 min ago'  },
    { icon: '🔖', text: 'Ben Tan borrowed "Clean Code"',                  time: '1 hour ago'  },
    { icon: '✅', text: 'Chitra returned "The Pragmatic Programmer"',      time: '2 hours ago' },
    { icon: '👤', text: 'New user David Lim registered',                  time: '3 hours ago' },
  ];

  // Get first letter for avatar
  getInitial(name: string): string {
    return name.charAt(0).toUpperCase();
  }
}