import { Pipe, PipeTransform } from '@angular/core';

@Pipe({ name: 'timeAgo', standalone: true, pure: false })
export class TimeAgoPipe implements PipeTransform {
  transform(value: string | Date): string {
    if (!value) return '';

    const date  = typeof value === 'string' ? new Date(value) : value;
    const now   = new Date();
    const secs  = Math.floor((now.getTime() - date.getTime()) / 1000);

    if (secs < 60)                     return 'Just now';
    if (secs < 3600)                   return `${Math.floor(secs / 60)}m ago`;
    if (secs < 86400)                  return `${Math.floor(secs / 3600)}h ago`;
    if (secs < 86400 * 2)              return 'Yesterday';
    if (secs < 86400 * 7)              return `${Math.floor(secs / 86400)}d ago`;
    if (secs < 86400 * 30)             return `${Math.floor(secs / (86400 * 7))}w ago`;
    return date.toLocaleDateString('en-GB', { day: '2-digit', month: 'short', year: 'numeric' });
  }
}
