import { Component, inject, Input, OnInit } from '@angular/core';
import { DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { Store } from '@ngrx/store';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ReviewService } from '../../../core/services';
import { Review, ProductRatingSummary } from '../../../core/models';
import { selectIsAuthenticated, selectUser } from '../../../store/auth/auth.selectors';
import { take } from 'rxjs';

@Component({
  selector: 'app-reviews',
  imports: [
    DatePipe, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatSelectModule
  ],
  templateUrl: './reviews.component.html',
  styleUrl: './reviews.component.scss'
})
export class ReviewsComponent implements OnInit {
  @Input() productId!: string;

  private reviewService = inject(ReviewService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);
  private store = inject(Store);

  Math = Math;
  reviews: Review[] = [];
  rating: ProductRatingSummary = { averageRating: 0, reviewCount: 0 };
  isAuthenticated = false;
  currentUserId = '';

  form = this.fb.group({
    rating: [5, [Validators.required, Validators.min(1), Validators.max(5)]],
    text: ['', Validators.required]
  });

  ngOnInit() {
    this.loadReviews();
    this.store.select(selectIsAuthenticated).pipe(take(1)).subscribe(auth => this.isAuthenticated = auth);
    this.store.select(selectUser).pipe(take(1)).subscribe(user => this.currentUserId = user?.id ?? '');
  }

  loadReviews() {
    this.reviewService.getReviews(this.productId).subscribe(reviews => this.reviews = reviews);
    this.reviewService.getRating(this.productId).subscribe(rating => this.rating = rating);
  }

  submitReview() {
    if (this.form.invalid) return;
    const { rating, text } = this.form.getRawValue();
    this.reviewService.createReview({ productId: this.productId, rating: rating!, text: text! }).subscribe(() => {
      this.snackBar.open('Review submitted!', '', { duration: 2000 });
      this.form.reset({ rating: 5, text: '' });
      this.loadReviews();
    });
  }

  deleteReview(id: string) {
    this.reviewService.deleteReview(id).subscribe(() => {
      this.snackBar.open('Review deleted.', '', { duration: 2000 });
      this.loadReviews();
    });
  }

  getStars(rating: number): number[] {
    return Array.from({ length: 5 }, (_, i) => i < rating ? 1 : 0);
  }
}
