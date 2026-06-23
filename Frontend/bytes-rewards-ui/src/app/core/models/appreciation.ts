export interface Appreciation {
  id: string;
  fromUserId: string;   // DB GUID — use for reliable sent/received detection
  toUserId: string;
  fromUserName: string;
  toUserName: string;
  message: string;
  createdAt: string;
  likesCount: number;
  isLiked: boolean;
}
