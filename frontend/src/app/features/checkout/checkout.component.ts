import { Component, inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { CartActions } from '../../store/cart/cart.actions';
import { selectCartItems, selectCartTotalPrice, selectCartLoading, selectCartUserName } from '../../store/cart/cart.selectors';
import { selectUser } from '../../store/auth/auth.selectors';
import { take } from 'rxjs';

@Component({
  selector: 'app-checkout',
  imports: [AsyncPipe, CurrencyPipe, ReactiveFormsModule, MatStepperModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent {
  private store = inject(Store);
  private fb = inject(FormBuilder);

  items$ = this.store.select(selectCartItems);
  totalPrice$ = this.store.select(selectCartTotalPrice);
  loading$ = this.store.select(selectCartLoading);

  shippingForm = this.fb.group({
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    emailAddress: ['', [Validators.required, Validators.email]],
    addressLine: ['', Validators.required],
    country: ['', Validators.required],
    state: ['', Validators.required],
    zipCode: ['', Validators.required]
  });

  paymentForm = this.fb.group({
    cardName: ['', Validators.required],
    cardNumber: ['', Validators.required],
    expiration: ['', Validators.required],
    cvv: ['', Validators.required]
  });

  placeOrder() {
    if (this.shippingForm.invalid || this.paymentForm.invalid) return;

    let userName = 'guest';
    let customerId = '';
    let totalPrice = 0;

    this.store.select(selectUser).pipe(take(1)).subscribe(user => {
      if (user) {
        userName = user.userName;
        customerId = user.id;
      }
    });

    this.store.select(selectCartTotalPrice).pipe(take(1)).subscribe(tp => totalPrice = tp);

    this.store.dispatch(CartActions.checkout({
      checkout: {
        userName,
        customerId,
        totalPrice,
        ...this.shippingForm.getRawValue() as any,
        ...this.paymentForm.getRawValue() as any,
        paymentMethod: 1
      }
    }));
  }
}
