import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class NotificationService {

  success(message: string): void {

    console.log('SUCCESS:', message);
  }

  error(message: string): void {

    console.log('ERROR:', message);
  }

  warning(message: string): void {

    console.log('WARNING:', message);
  }
}