export interface RewardCategory {
  id: string;
  name: string;
  description: string;
  bytes: number;
  isActive: boolean;
}

export interface CreateRewardCategoryRequest {
  name: string;
  description: string;
  bytes: number;
}
