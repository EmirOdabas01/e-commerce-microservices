import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { WishlistService } from '../../core/services';
import { WishlistItem } from '../../core/models';

@Component({
  selector: 'app-wishlist',
  imports: [RouterLink, CurrencyPipe, MatCardModule, MatButtonModule, MatIconModule],
  templateUrl: './wishlist.component.html',
  styleUrl: './wishlist.component.scss'
})
export class WishlistComponent implements OnInit {
  private wishlistService = inject(WishlistService);
  private snackBar = inject(MatSnackBar);

  items: WishlistItem[] = [];
  loading = false;

  ngOnInit() {
    this.loadWishlist();
  }

  loadWishlist() {
    this.loading = true;
    this.wishlistService.getWishlist().subscribe({
      next: items => { this.items = items; this.loading = false; },
      error: () => { this.loading = false; this.snackBar.open('Failed to load wishlist.', '', { duration: 3000 }); }
    });
  }

  remove(productId: string) {
    this.wishlistService.removeFromWishlist(productId).subscribe({
      next: () => {
        this.snackBar.open('Removed from wishlist.', '', { duration: 2000 });
        this.loadWishlist();
      },
      error: () => this.snackBar.open('Failed to remove item.', '', { duration: 3000 })
    });
  }
}
