import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Category, CategoryRequest } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CategoryService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/categories`;

  getCategories(): Observable<Category[]> {
    return this.http.get<Category[]>(this.url);
  }

  createCategory(request: CategoryRequest): Observable<Category> {
    return this.http.post<Category>(this.url, request);
  }

  updateCategory(id: string, request: CategoryRequest): Observable<Category> {
    return this.http.put<Category>(`${this.url}/${id}`, request);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
