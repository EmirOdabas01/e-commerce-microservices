import { Component, inject, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSnackBar } from '@angular/material/snack-bar';
import { ProductActions } from '../../store/product/product.actions';
import { CartActions } from '../../store/cart/cart.actions';
import { selectSelectedProduct, selectProductLoading } from '../../store/product/product.selectors';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-product-detail',
  imports: [
    AsyncPipe, CurrencyPipe, FormsModule, MatCardModule, MatButtonModule,
    MatChipsModule, MatFormFieldModule, MatInputModule, MatIconModule, LoadingSpinnerComponent
  ],
  templateUrl: './product-detail.component.html',
  styleUrl: './product-detail.component.scss'
})
export class ProductDetailComponent implements OnInit {
  private store = inject(Store);
  private route = inject(ActivatedRoute);
  private snackBar = inject(MatSnackBar);

  product$ = this.store.select(selectSelectedProduct);
  loading$ = this.store.select(selectProductLoading);
  quantity = 1;
  selectedImageIndex = 0;

  ngOnInit() {
    const id = this.route.snapshot.paramMap.get('id')!;
    this.store.dispatch(ProductActions.loadProduct({ id }));
  }

  selectImage(index: number) {
    this.selectedImageIndex = index;
  }

  prevImage(total: number) {
    this.selectedImageIndex = (this.selectedImageIndex - 1 + total) % total;
  }

  nextImage(total: number) {
    this.selectedImageIndex = (this.selectedImageIndex + 1) % total;
  }

  addToCart(product: any) {
    this.store.dispatch(CartActions.addToCart({
      item: {
        productId: product.id,
        productName: product.name,
        price: product.price,
        quantity: this.quantity
      }
    }));
    this.snackBar.open('Added to cart!', 'Close', { duration: 2000 });
  }
}
