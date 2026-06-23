export interface RedemptionHistory {
  redemptionId: string;
  productName: string;
  redeemedBytes: number;
  status: 'Pending' | 'Approved' | 'Rejected' | 'Delivered';
  redeemedAt: string;
}
