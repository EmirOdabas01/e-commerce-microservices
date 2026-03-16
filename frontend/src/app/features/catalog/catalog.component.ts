import { Component, inject, OnInit } from '@angular/core';
import { RouterLink } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, SlicePipe } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatPaginatorModule, PageEvent } from '@angular/material/paginator';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatIconModule } from '@angular/material/icon';
import { MatSelectModule } from '@angular/material/select';
import { ProductActions } from '../../store/product/product.actions';
import { selectAllProducts, selectProductLoading, selectProductTotalCount, selectProductPageIndex, selectProductPageSize } from '../../store/product/product.selectors';
import { CategoryService } from '../../core/services';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-catalog',
  imports: [
    RouterLink, AsyncPipe, CurrencyPipe, SlicePipe, FormsModule,
    MatCardModule, MatButtonModule, MatChipsModule, MatPaginatorModule,
    MatFormFieldModule, MatInputModule, MatIconModule, MatSelectModule,
    LoadingSpinnerComponent
  ],
  templateUrl: './catalog.component.html',
  styleUrl: './catalog.component.scss'
})
export class CatalogComponent implements OnInit {
  private store = inject(Store);
  private categoryService = inject(CategoryService);

  products$ = this.store.select(selectAllProducts);
  loading$ = this.store.select(selectProductLoading);
  totalCount$ = this.store.select(selectProductTotalCount);
  pageIndex$ = this.store.select(selectProductPageIndex);
  pageSize$ = this.store.select(selectProductPageSize);

  selectedCategory = '';
  searchQuery = '';
  minPrice: number | null = null;
  maxPrice: number | null = null;
  sortBy = '';
  sortOrder = 'asc';
  categories: string[] = [];

  private get filters() {
    return {
      minPrice: this.minPrice ?? undefined,
      maxPrice: this.maxPrice ?? undefined,
      sortBy: this.sortBy || undefined,
      sortOrder: this.sortBy ? this.sortOrder : undefined
    };
  }

  ngOnInit() {
    this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10 }));
    this.categoryService.getCategories().subscribe(cats => {
      this.categories = cats.map(c => c.name);
    });
  }

  onSearch(query: string) {
    this.searchQuery = query.trim();
    this.selectedCategory = '';
    if (this.searchQuery) {
      this.store.dispatch(ProductActions.searchProducts({ query: this.searchQuery, pageIndex: 0, pageSize: 10 }));
    } else {
      this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10, filters: this.filters }));
    }
  }

  applyFilters() {
    this.searchQuery = '';
    this.selectedCategory = '';
    this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10, filters: this.filters }));
  }

  clearFilters() {
    this.minPrice = null;
    this.maxPrice = null;
    this.sortBy = '';
    this.sortOrder = 'asc';
    this.searchQuery = '';
    this.selectedCategory = '';
    this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10 }));
  }

  onPageChange(event: PageEvent) {
    if (this.searchQuery) {
      this.store.dispatch(ProductActions.searchProducts({ query: this.searchQuery, pageIndex: event.pageIndex, pageSize: event.pageSize }));
    } else if (this.selectedCategory) {
      this.store.dispatch(ProductActions.loadProductsByCategory({ category: this.selectedCategory }));
    } else {
      this.store.dispatch(ProductActions.loadProducts({ pageIndex: event.pageIndex, pageSize: event.pageSize, filters: this.filters }));
    }
  }

  filterByCategory(category: string) {
    this.searchQuery = '';
    if (this.selectedCategory === category) {
      this.selectedCategory = '';
      this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 10, filters: this.filters }));
    } else {
      this.selectedCategory = category;
      this.store.dispatch(ProductActions.loadProductsByCategory({ category }));
    }
  }
}
