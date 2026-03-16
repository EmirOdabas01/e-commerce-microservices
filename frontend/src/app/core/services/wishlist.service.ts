import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { WishlistItem } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class WishlistService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/wishlist`;

  getWishlist(): Observable<WishlistItem[]> {
    return this.http.get<WishlistItem[]>(this.url);
  }

  addToWishlist(productId: string): Observable<void> {
    return this.http.post<void>(`${this.url}/${productId}`, {});
  }

  removeFromWishlist(productId: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${productId}`);
  }
}
