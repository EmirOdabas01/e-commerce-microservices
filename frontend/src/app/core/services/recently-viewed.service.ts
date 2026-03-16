import { Injectable } from '@angular/core';
import { Product } from '../models';

@Injectable({ providedIn: 'root' })
export class RecentlyViewedService {
  private readonly key = 'recently_viewed';
  private readonly maxItems = 10;

  getRecentlyViewed(): Product[] {
    try {
      const stored = localStorage.getItem(this.key);
      return stored ? JSON.parse(stored) : [];
    } catch {
      return [];
    }
  }

  addProduct(product: Product) {
    const items = this.getRecentlyViewed().filter(p => p.id !== product.id);
    items.unshift(product);
    localStorage.setItem(this.key, JSON.stringify(items.slice(0, this.maxItems)));
  }
}
