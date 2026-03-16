import { Injectable, inject } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { OrderDto, PaginatedResult } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class OrderService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/orders`;

  getOrders(pageIndex = 0, pageSize = 10): Observable<PaginatedResult<OrderDto>> {
    const params = new HttpParams()
      .set('PageNumber', pageIndex + 1)
      .set('PageSize', pageSize);
    return this.http.get<PaginatedResult<OrderDto>>(this.url, { params });
  }

  getOrdersByUser(userName: string): Observable<OrderDto[]> {
    return this.http.get<OrderDto[]>(`${this.url}/user/${userName}`);
  }

  cancelOrder(id: string): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}/cancel`, {});
  }

  refundOrder(id: string): Observable<void> {
    return this.http.put<void>(`${this.url}/${id}/refund`, {});
  }
}
