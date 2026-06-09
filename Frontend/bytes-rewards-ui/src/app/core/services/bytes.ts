import { Injectable } from '@angular/core';

@Injectable({
  providedIn: 'root'
})
export class BytesService {

  private totalBytes = 4500;

  getBalance(): number {

    return this.totalBytes;
  }

  addBytes(amount: number): void {

    this.totalBytes += amount;
  }

  deductBytes(amount: number): void {

    this.totalBytes -= amount;
  }
}