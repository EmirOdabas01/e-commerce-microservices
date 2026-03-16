import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, SlicePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { selectUser, selectAuthLoading } from '../../store/auth/auth.selectors';
import { selectAllOrders, selectOrderLoading } from '../../store/order/order.selectors';
import { OrderActions } from '../../store/order/order.actions';
import { AuthActions } from '../../store/auth/auth.actions';
import { OrderStatus } from '../../core/models';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { AddressManagementComponent } from './address-management/address-management.component';
import { PaymentMethodsComponent } from './payment-methods/payment-methods.component';
import { take } from 'rxjs';

@Component({
  selector: 'app-profile',
  imports: [
    AsyncPipe, CurrencyPipe, SlicePipe, ReactiveFormsModule,
    MatCardModule, MatTableModule, MatChipsModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, LoadingSpinnerComponent,
    AddressManagementComponent, PaymentMethodsComponent
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private store = inject(Store);
  private fb = inject(FormBuilder);

  user$ = this.store.select(selectUser);
  orders$ = this.store.select(selectAllOrders);
  loading$ = this.store.select(selectOrderLoading);
  profileLoading$ = this.store.select(selectAuthLoading);
  displayedColumns = ['id', 'status', 'totalPrice', 'actions'];
  OrderStatus = OrderStatus;
  editing = false;

  profileForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]]
  });

  getStatusLabel(status: OrderStatus): string {
    return OrderStatus[status] ?? 'Unknown';
  }

  startEdit() {
    this.user$.pipe(take(1)).subscribe(user => {
      if (user) {
        this.profileForm.patchValue({
          firstName: user.firstName,
          lastName: user.lastName,
          email: user.email
        });
      }
    });
    this.editing = true;
  }

  cancelEdit() {
    this.editing = false;
  }

  saveProfile() {
    if (this.profileForm.valid) {
      this.store.dispatch(AuthActions.updateProfile({
        request: this.profileForm.getRawValue() as { firstName: string; lastName: string; email: string }
      }));
      this.editing = false;
    }
  }

  cancelOrder(id: string) {
    this.user$.pipe(take(1)).subscribe(user => {
      if (user) this.store.dispatch(OrderActions.cancelOrder({ id, userName: user.userName }));
    });
  }

  refundOrder(id: string) {
    this.user$.pipe(take(1)).subscribe(user => {
      if (user) this.store.dispatch(OrderActions.refundOrder({ id, userName: user.userName }));
    });
  }

  ngOnInit() {
    this.store.select(selectUser).pipe(take(1)).subscribe(user => {
      if (user) {
        this.store.dispatch(OrderActions.loadUserOrders({ userName: user.userName }));
      }
    });
  }
}
