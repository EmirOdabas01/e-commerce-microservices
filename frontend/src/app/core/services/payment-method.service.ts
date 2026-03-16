import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { PaymentMethodResponse, PaymentMethodRequest } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class PaymentMethodService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/identity/payment-methods`;

  getPaymentMethods(): Observable<PaymentMethodResponse[]> {
    return this.http.get<PaymentMethodResponse[]>(this.url);
  }

  createPaymentMethod(request: PaymentMethodRequest): Observable<PaymentMethodResponse> {
    return this.http.post<PaymentMethodResponse>(this.url, request);
  }

  deletePaymentMethod(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
