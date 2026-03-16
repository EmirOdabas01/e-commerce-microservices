import { Component, inject, OnInit } from '@angular/core';
import { NgClass } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatCheckboxModule } from '@angular/material/checkbox';
import { MatSnackBar } from '@angular/material/snack-bar';
import { PaymentMethodService } from '../../../core/services';
import { PaymentMethodResponse } from '../../../core/models';

@Component({
  selector: 'app-payment-methods',
  imports: [
    NgClass, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatCheckboxModule
  ],
  templateUrl: './payment-methods.component.html',
  styleUrl: './payment-methods.component.scss'
})
export class PaymentMethodsComponent implements OnInit {
  private paymentMethodService = inject(PaymentMethodService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  methods: PaymentMethodResponse[] = [];
  adding = false;

  form = this.fb.group({
    label: ['', Validators.required],
    cardName: ['', Validators.required],
    cardNumber: ['', [Validators.required, Validators.minLength(13)]],
    expiration: ['', Validators.required],
    isDefault: [false]
  });

  ngOnInit() {
    this.loadMethods();
  }

  loadMethods() {
    this.paymentMethodService.getPaymentMethods().subscribe({
      next: methods => this.methods = methods,
      error: () => this.snackBar.open('Failed to load payment methods.', '', { duration: 3000 })
    });
  }

  openAdd() {
    this.form.reset({ isDefault: false });
    this.adding = true;
  }

  cancel() {
    this.adding = false;
  }

  save() {
    if (this.form.invalid) return;
    this.paymentMethodService.createPaymentMethod(this.form.getRawValue() as any).subscribe({
      next: () => {
        this.snackBar.open('Payment method added!', '', { duration: 2000 });
        this.adding = false;
        this.loadMethods();
      },
      error: () => this.snackBar.open('Failed to add payment method.', '', { duration: 3000 })
    });
  }

  delete(id: string) {
    this.paymentMethodService.deletePaymentMethod(id).subscribe({
      next: () => {
        this.snackBar.open('Payment method removed.', '', { duration: 2000 });
        this.loadMethods();
      },
      error: () => this.snackBar.open('Failed to delete payment method.', '', { duration: 3000 })
    });
  }
}
