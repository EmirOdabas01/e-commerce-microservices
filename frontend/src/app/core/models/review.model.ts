export interface Review {
  id: string;
  productId: string;
  userId: string;
  userName: string;
  rating: number;
  text: string;
  createdAt: string;
}

export interface CreateReviewRequest {
  productId: string;
  rating: number;
  text: string;
}

export interface ProductRatingSummary {
  averageRating: number;
  reviewCount: number;
}
