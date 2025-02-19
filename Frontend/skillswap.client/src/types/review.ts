export interface IReview {
    id?: string;
    sessionId: string;
    reviewerId: string;
    rating: number;
    comments: string;
    createdAt?: string;
}