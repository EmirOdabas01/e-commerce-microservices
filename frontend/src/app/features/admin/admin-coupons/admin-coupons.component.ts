import { Component, inject, OnInit } from '@angular/core';
import { CurrencyPipe, DatePipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CouponService } from '../../../core/services';
import { Coupon } from '../../../core/models';

@Component({
  selector: 'app-admin-coupons',
  imports: [
    CurrencyPipe, DatePipe, ReactiveFormsModule, MatTableModule,
    MatButtonModule, MatIconModule, MatFormFieldModule, MatInputModule, MatCardModule
  ],
  templateUrl: './admin-coupons.component.html',
  styleUrl: './admin-coupons.component.scss'
})
export class AdminCouponsComponent implements OnInit {
  private couponService = inject(CouponService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  coupons: Coupon[] = [];
  displayedColumns = ['code', 'discount', 'minOrder', 'usage', 'expires', 'actions'];
  showForm = false;

  form = this.fb.group({
    code: ['', Validators.required],
    discountPercent: [10, [Validators.required, Validators.min(1), Validators.max(100)]],
    maxDiscountAmount: [null as number | null],
    minOrderAmount: [0, Validators.required],
    usageLimit: [0, Validators.required],
    expiresAt: ['', Validators.required]
  });

  ngOnInit() {
    this.loadCoupons();
  }

  loadCoupons() {
    this.couponService.getCoupons().subscribe(coupons => this.coupons = coupons);
  }

  openCreate() {
    this.form.reset({ discountPercent: 10, minOrderAmount: 0, usageLimit: 0 });
    this.showForm = true;
  }

  cancel() {
    this.showForm = false;
  }

  save() {
    if (this.form.invalid) return;
    const value = this.form.getRawValue();
    this.couponService.createCoupon({
      code: value.code!,
      discountPercent: value.discountPercent!,
      maxDiscountAmount: value.maxDiscountAmount,
      minOrderAmount: value.minOrderAmount!,
      usageLimit: value.usageLimit!,
      expiresAt: new Date(value.expiresAt!).toISOString()
    }).subscribe(() => {
      this.snackBar.open('Coupon created!', '', { duration: 2000 });
      this.showForm = false;
      this.loadCoupons();
    });
  }

  deleteCoupon(id: string) {
    if (confirm('Delete this coupon?')) {
      this.couponService.deleteCoupon(id).subscribe(() => {
        this.snackBar.open('Coupon deleted.', '', { duration: 2000 });
        this.loadCoupons();
      });
    }
  }
}
