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

  ngOnInit() {
    this.loadWishlist();
  }

  loadWishlist() {
    this.wishlistService.getWishlist().subscribe(items => this.items = items);
  }

  remove(productId: string) {
    this.wishlistService.removeFromWishlist(productId).subscribe(() => {
      this.snackBar.open('Removed from wishlist.', '', { duration: 2000 });
      this.loadWishlist();
    });
  }
}
