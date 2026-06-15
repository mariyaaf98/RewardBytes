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
}
