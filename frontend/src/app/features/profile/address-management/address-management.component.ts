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
import { AddressService } from '../../../core/services';
import { Address } from '../../../core/models';

@Component({
  selector: 'app-address-management',
  imports: [
    NgClass, ReactiveFormsModule, MatCardModule, MatFormFieldModule,
    MatInputModule, MatButtonModule, MatIconModule, MatCheckboxModule
  ],
  templateUrl: './address-management.component.html',
  styleUrl: './address-management.component.scss'
})
export class AddressManagementComponent implements OnInit {
  private addressService = inject(AddressService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  addresses: Address[] = [];
  editing = false;
  editingId: string | null = null;

  form = this.fb.group({
    label: ['', Validators.required],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    addressLine: ['', Validators.required],
    country: ['', Validators.required],
    state: ['', Validators.required],
    zipCode: ['', Validators.required],
    emailAddress: ['', [Validators.required, Validators.email]],
    isDefault: [false]
  });

  ngOnInit() {
    this.loadAddresses();
  }

  loadAddresses() {
    this.addressService.getAddresses().subscribe({
      next: addresses => this.addresses = addresses,
      error: () => this.snackBar.open('Failed to load addresses.', '', { duration: 3000 })
    });
  }

  openCreate() {
    this.form.reset({ isDefault: false });
    this.editingId = null;
    this.editing = true;
  }

  openEdit(address: Address) {
    this.form.patchValue(address);
    this.editingId = address.id;
    this.editing = true;
  }

  cancel() {
    this.editing = false;
    this.editingId = null;
  }

  save() {
    if (this.form.invalid) return;
    const request = this.form.getRawValue() as any;

    if (this.editingId) {
      this.addressService.updateAddress(this.editingId, request).subscribe({
        next: () => {
          this.snackBar.open('Address updated!', '', { duration: 2000 });
          this.editing = false;
          this.editingId = null;
          this.loadAddresses();
        },
        error: () => this.snackBar.open('Failed to update address.', '', { duration: 3000 })
      });
    } else {
      this.addressService.createAddress(request).subscribe({
        next: () => {
          this.snackBar.open('Address added!', '', { duration: 2000 });
          this.editing = false;
          this.loadAddresses();
        },
        error: () => this.snackBar.open('Failed to add address.', '', { duration: 3000 })
      });
    }
  }

  delete(id: string) {
    this.addressService.deleteAddress(id).subscribe({
      next: () => {
        this.snackBar.open('Address removed.', '', { duration: 2000 });
        this.loadAddresses();
      },
      error: () => this.snackBar.open('Failed to delete address.', '', { duration: 3000 })
    });
  }
}
