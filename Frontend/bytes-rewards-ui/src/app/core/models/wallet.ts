export interface WalletResponse {
  availableBytes: number;
}

export interface LedgerEntry {
  rewardId: string;
  rewardCategoryName: string;
  bytes: number;
  awardedBy: string;
  reason: string;
  awardedAt: string;
  entryType: 'Reward' | 'Refund';
}

// Unified transaction for the full history view
export interface Transaction {
  id: string;
  type: 'credit' | 'debit';
  title: string;
  subtitle: string;
  note: string;
  bytes: number;       // always positive
  date: string;
  runningBalance?: number;
}
