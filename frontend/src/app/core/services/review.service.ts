import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Review, CreateReviewRequest, ProductRatingSummary } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ReviewService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/products`;

  getReviews(productId: string): Observable<Review[]> {
    return this.http.get<Review[]>(`${this.url}/${productId}/reviews`);
  }

  getRating(productId: string): Observable<ProductRatingSummary> {
    return this.http.get<ProductRatingSummary>(`${this.url}/${productId}/rating`);
  }

  createReview(request: CreateReviewRequest): Observable<Review> {
    return this.http.post<Review>(`${this.url}/reviews`, request);
  }

  deleteReview(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/reviews/${id}`);
  }

  reportReview(id: string): Observable<void> {
    return this.http.post<void>(`${this.url}/reviews/${id}/report`, {});
  }
}
