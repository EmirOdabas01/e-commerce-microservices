import { Injectable, inject } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Address, AddressRequest } from '../models';
import { environment } from '../../../environments/environment';

@Injectable({ providedIn: 'root' })
export class AddressService {
  private http = inject(HttpClient);
  private url = `${environment.apiUrl}/api/identity/addresses`;

  getAddresses(): Observable<Address[]> {
    return this.http.get<Address[]>(this.url);
  }

  createAddress(request: AddressRequest): Observable<Address> {
    return this.http.post<Address>(this.url, request);
  }

  updateAddress(id: string, request: AddressRequest): Observable<Address> {
    return this.http.put<Address>(`${this.url}/${id}`, request);
  }

  deleteAddress(id: string): Observable<void> {
    return this.http.delete<void>(`${this.url}/${id}`);
  }
}
