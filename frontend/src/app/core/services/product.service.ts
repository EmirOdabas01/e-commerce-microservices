import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Product, PaginatedResult } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class ProductService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/products`;

  getProducts(pageIndex = 0, pageSize = 10, filters?: { minPrice?: number; maxPrice?: number; sortBy?: string; sortOrder?: string }): Observable<PaginatedResult<Product>> {
    let params = new HttpParams()
      .set('PageNumber', pageIndex + 1)
      .set('PageSize', pageSize);
    if (filters?.minPrice != null) params = params.set('MinPrice', filters.minPrice);
    if (filters?.maxPrice != null) params = params.set('MaxPrice', filters.maxPrice);
    if (filters?.sortBy) params = params.set('SortBy', filters.sortBy);
    if (filters?.sortOrder) params = params.set('SortOrder', filters.sortOrder);
    return this.http.get<PaginatedResult<Product>>(this.url, { params });
  }

  searchProducts(query: string, pageIndex = 0, pageSize = 10): Observable<PaginatedResult<Product>> {
    const params = new HttpParams()
      .set('Query', query)
      .set('PageNumber', pageIndex + 1)
      .set('PageSize', pageSize);
    return this.http.get<PaginatedResult<Product>>(`${this.url}/search`, { params });
  }

  getProduct(id: string): Observable<Product> {
    return this.http.get<Product>(`${this.url}/${id}`);
  }

  getProductsByCategory(category: string): Observable<Product[]> {
    return this.http.get<Product[]>(`${this.url}/category/${category}`);
  }

  createProduct(product: Product): Observable<Product> {
    return this.http.post<Product>(this.url, product);
  }

  updateProduct(product: Product): Observable<void> {
    return this.http.put<void>(this.url, product);
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
