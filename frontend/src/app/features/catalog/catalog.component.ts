import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, SlicePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { ProductActions } from '../../store/product/product.actions';
import { selectAllProducts, selectProductLoading, selectProductTotalCount, selectProductPageIndex, selectProductPageSize } from '../../store/product/product.selectors';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-catalog',
  imports: [RouterLink, AsyncPipe, CurrencyPipe, SlicePipe, MatCardModule, MatButtonModule, MatChipsModule, MatPaginatorModule, LoadingSpinnerComponent],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss'
})
export class CatalogComponent implements OnInit {
  private store = inject(Store);

  products$ = this.store.select(selectAllProducts);
  loading$ = this.store.select(selectProductLoading);
  totalCount$ = this.store.select(selectProductTotalCount);
  pageIndex$ = this.store.select(selectProductPageIndex);
  pageSize$ = this.store.select(selectProductPageSize);

  selectedCategory = '';
  categories = ['Smart Phone', 'White Appliances', 'Home Kitchen', 'Camera'];

  ngOnInit() {
    this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10 }));
  }

  onPageChange(event: PageEvent) {
    if (this.selectedCategory) {
      this.store.dispatch(ProductActions.loadProductsByCategory({ category: this.selectedCategory }));
    } else {
      this.store.dispatch(ProductActions.loadProducts({ pageIndex: event.pageIndex, pageSize: event.pageSize }));
    }
  }

  filterByCategory(category: string) {
    if (this.selectedCategory === category) {
      this.selectedCategory = '';
      this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10 }));
    } else {
      this.selectedCategory = category;
      this.store.dispatch(ProductActions.loadProductsByCategory({ category }));
    }
  }
}
