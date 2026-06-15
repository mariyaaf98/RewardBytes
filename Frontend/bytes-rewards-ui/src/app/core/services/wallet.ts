import { inject, Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WalletResponse, LedgerEntry } from '../models/wallet';

@Injectable({
  providedIn: 'root'
})
export class WalletService {

  private readonly http = inject(HttpClient);

  private readonly baseUrl = 'http://localhost:7000';

  getWallet(userId: string): Observable<WalletResponse> {
    return this.http.get<WalletResponse>(
      `${this.baseUrl}/wallets/${userId}`
    );
  }

  getWalletLedger(userId: string): Observable<LedgerEntry[]> {
    return this.http.get<LedgerEntry[]>(
      `${this.baseUrl}/wallets/ledger/${userId}`
    );
  }
}
