import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { ShoppingCart, BasketCheckout } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class BasketService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/basket`;

  getBasket(userName: string): Observable<ShoppingCart> {
    return this.http.get<ShoppingCart>(`${this.url}/${userName}`);
  }

  storeBasket(cart: ShoppingCart): Observable<ShoppingCart> {
    return this.http.post<ShoppingCart>(this.url, cart);
  }

  deleteBasket(userName: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${userName}`);
  }

  checkout(checkout: BasketCheckout): Observable<void> {
    return this.http.post<void>(`${this.url}/checkout`, checkout);
  }
}
