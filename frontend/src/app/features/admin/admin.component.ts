import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe } from '@angular/common';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatDialogModule } from '@angular/material/dialog';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatTabsModule } from '@angular/material/tabs';
import { ProductActions } from '../../store/product/product.actions';
import { selectAllProducts, selectProductLoading } from '../../store/product/product.selectors';
import { Product } from '../../core/models';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { AdminOrdersComponent } from './admin-orders/admin-orders.component';
import { AdminUsersComponent } from './admin-users/admin-users.component';
import { AdminDashboardComponent } from './admin-dashboard/admin-dashboard.component';
import { AdminCategoriesComponent } from './admin-categories/admin-categories.component';
import { AdminCouponsComponent } from './admin-coupons/admin-coupons.component';

@Component({
  selector: 'app-admin',
  imports: [
    AsyncPipe, CurrencyPipe, ReactiveFormsModule,
    MatTableModule, MatButtonModule, MatIconModule, MatDialogModule,
    MatFormFieldModule, MatInputModule, MatCardModule, MatTabsModule,
    LoadingSpinnerComponent, AdminOrdersComponent, AdminUsersComponent,
    AdminDashboardComponent, AdminCategoriesComponent, AdminCouponsComponent
  ],
  templateUrl: './admin.component.html',
  styleUrl: './admin.component.scss'
})
export class AdminComponent implements OnInit {
  private store = inject(Store);
  private fb = inject(FormBuilder);

  products$ = this.store.select(selectAllProducts);
  loading$ = this.store.select(selectProductLoading);
  displayedColumns = ['name', 'category', 'price', 'stock', 'actions'];

  showForm = false;
  editingProduct: Product | null = null;

  form = this.fb.group({
    id: [''],
    name: ['', Validators.required],
    category: ['', Validators.required],
    description: ['', Validators.required],
    imageFile: ['', Validators.required],
    price: [0, [Validators.required, Validators.min(0.01)]],
    stock: [0, [Validators.required, Validators.min(0)]]
  });

  ngOnInit() {
    this.store.dispatch(ProductActions.loadProducts({ pageIndex: 0, pageSize: 100 }));
  }

  openCreate() {
    this.editingProduct = null;
    this.form.reset();
    this.showForm = true;
  }

  openEdit(product: Product) {
    this.editingProduct = product;
    this.form.patchValue({
      ...product,
      category: product.category.join(', ')
    });
    this.showForm = true;
  }

  cancel() {
    this.showForm = false;
    this.editingProduct = null;
  }

  save() {
    if (this.form.invalid) return;

    const formValue = this.form.getRawValue();
    const product: Product = {
      id: formValue.id || crypto.randomUUID(),
      name: formValue.name!,
      category: formValue.category!.split(',').map(c => c.trim()),
      description: formValue.description!,
      imageFile: formValue.imageFile!,
      imageFiles: [formValue.imageFile!],
      price: formValue.price!,
      stock: formValue.stock!
    };

    if (this.editingProduct) {
      this.store.dispatch(ProductActions.updateProduct({ product }));
    } else {
      this.store.dispatch(ProductActions.createProduct({ product }));
    }
    this.showForm = false;
    this.editingProduct = null;
  }

  deleteProduct(id: string) {
    if (confirm('Are you sure you want to delete this product?')) {
      this.store.dispatch(ProductActions.deleteProduct({ id }));
    }
  }
}
