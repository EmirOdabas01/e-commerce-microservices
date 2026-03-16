import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatStepperModule } from '@angular/material/stepper';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSelectModule } from '@angular/material/select';
import { MatRadioModule } from '@angular/material/radio';
import { CartActions } from '../../store/cart/cart.actions';
import { selectCartItems, selectCartTotalPrice, selectCartLoading } from '../../store/cart/cart.selectors';
import { selectUser } from '../../store/auth/auth.selectors';
import { AddressService, PaymentMethodService } from '../../core/services';
import { Address, PaymentMethodResponse } from '../../core/models';
import { take } from 'rxjs';

export interface ShippingMethod {
  id: string;
  label: string;
  price: number;
  estimatedDays: string;
}

@Component({
  selector: 'app-checkout',
  imports: [
    AsyncPipe, CurrencyPipe, ReactiveFormsModule, MatStepperModule,
    MatFormFieldModule, MatInputModule, MatButtonModule, MatSelectModule,
    MatRadioModule
  ],
  templateUrl: './checkout.component.html',
  styleUrl: './checkout.component.scss'
})
export class CheckoutComponent implements OnInit {
  private store = inject(Store);
  private fb = inject(FormBuilder);
  private addressService = inject(AddressService);
  private paymentMethodService = inject(PaymentMethodService);

  items$ = this.store.select(selectCartItems);
  totalPrice$ = this.store.select(selectCartTotalPrice);
  loading$ = this.store.select(selectCartLoading);

  savedAddresses: Address[] = [];
  savedPaymentMethods: PaymentMethodResponse[] = [];

  shippingMethods: ShippingMethod[] = [
    { id: 'standard', label: 'Standard Shipping', price: 5.99, estimatedDays: '5-7 business days' },
    { id: 'express', label: 'Express Shipping', price: 14.99, estimatedDays: '2-3 business days' },
    { id: 'overnight', label: 'Overnight Shipping', price: 29.99, estimatedDays: '1 business day' }
  ];
  selectedShippingMethod: ShippingMethod = this.shippingMethods[0];

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

  ngOnInit() {
    this.addressService.getAddresses().subscribe(addresses => {
      this.savedAddresses = addresses;
      const defaultAddr = addresses.find(a => a.isDefault);
      if (defaultAddr) this.selectAddress(defaultAddr);
    });
    this.paymentMethodService.getPaymentMethods().subscribe(methods => {
      this.savedPaymentMethods = methods;
    });
  }

  selectAddress(address: Address) {
    this.shippingForm.patchValue({
      firstName: address.firstName,
      lastName: address.lastName,
      emailAddress: address.emailAddress,
      addressLine: address.addressLine,
      country: address.country,
      state: address.state,
      zipCode: address.zipCode
    });
  }

  selectPaymentMethod(method: PaymentMethodResponse) {
    this.paymentForm.patchValue({
      cardName: method.cardName,
      expiration: method.expiration
    });
  }

  onShippingMethodChange(method: ShippingMethod) {
    this.selectedShippingMethod = method;
  }

  getOrderTotal(cartTotal: number): number {
    return cartTotal + this.selectedShippingMethod.price;
  }

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
        totalPrice: totalPrice + this.selectedShippingMethod.price,
        ...this.shippingForm.getRawValue() as any,
        ...this.paymentForm.getRawValue() as any,
        paymentMethod: 1
      }
    }));
  }
}
