import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { ProductActions } from '../../store/product/product.actions';
import { selectAllProducts, selectProductLoading } from '../../store/product/product.selectors';
import { RecentlyViewedService } from '../../core/services';
import { Product } from '../../core/models';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-home',
  imports: [RouterLink, AsyncPipe, CurrencyPipe, MatCardModule, MatButtonModule, LoadingSpinnerComponent],
  templateUrl: './home.component.html',
  styleUrl: './home.component.scss'
})
export class HomeComponent implements OnInit {
  private store = inject(Store);
  private recentlyViewedService = inject(RecentlyViewedService);

  products$ = this.store.select(selectAllProducts);
  loading$ = this.store.select(selectProductLoading);
  recentlyViewed: Product[] = [];

  ngOnInit() {
    this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 8 }));
    this.recentlyViewed = this.recentlyViewedService.getRecentlyViewed();
  }
}
