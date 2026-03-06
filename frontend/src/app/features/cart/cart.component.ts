import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { CartActions } from '../../store/cart/cart.actions';
import { selectCartItems, selectCartTotalPrice, selectCartLoading } from '../../store/cart/cart.selectors';
import { selectIsAuthenticated } from '../../store/auth/auth.selectors';
import { ShoppingCartItem } from '../../core/models';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-cart',
  imports: [RouterLink, AsyncPipe, CurrencyPipe, MatTableModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent],
  templateUrl: './cart.component.html',
  styleUrl: './cart.component.scss'
})
export class CartComponent {
  private store = inject(Store);

  items$ = this.store.select(selectCartItems);
  totalPrice$ = this.store.select(selectCartTotalPrice);
  loading$ = this.store.select(selectCartLoading);
  isAuthenticated$ = this.store.select(selectIsAuthenticated);
  displayedColumns = ['product', 'price', 'quantity', 'subtotal', 'actions'];

  updateQuantity(item: ShoppingCartItem, delta: number) {
    const newQty = item.quantity + delta;
    if (newQty > 0) {
      this.store.dispatch(CartActions.updateQuantity({ productId: item.productId, quantity: newQty }));
    }
  }

  removeItem(productId: string) {
    this.store.dispatch(CartActions.removeFromCart({ productId }));
  }
}
