import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Coupon, CouponRequest, ValidateCouponResponse } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class CouponService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/coupons`;

  getCoupons(): Observable<Coupon[]> {
    return this.http.get<Coupon[]>(this.url);
  }

  createCoupon(request: CouponRequest): Observable<Coupon> {
    return this.http.post<Coupon>(this.url, request);
  }

  deleteCoupon(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }

  validateCoupon(code: string, orderTotal: number): Observable<ValidateCouponResponse> {
    return this.http.post<ValidateCouponResponse>(`${this.url}/validate`, { code, orderTotal });
  }
}
