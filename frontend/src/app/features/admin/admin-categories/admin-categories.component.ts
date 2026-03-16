import { Component, inject, OnInit } from '@angular/core';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatCardModule } from '@angular/material/card';
import { MatSnackBar } from '@angular/material/snack-bar';
import { CategoryService } from '../../../core/services';
import { Category } from '../../../core/models';

@Component({
  selector: 'app-admin-categories',
  imports: [
    ReactiveFormsModule, MatTableModule, MatButtonModule, MatIconModule,
    MatFormFieldModule, MatInputModule, MatCardModule
  ],
  templateUrl: './admin-categories.component.html',
  styleUrl: './admin-categories.component.scss'
})
export class AdminCategoriesComponent implements OnInit {
  private categoryService = inject(CategoryService);
  private fb = inject(FormBuilder);
  private snackBar = inject(MatSnackBar);

  categories: Category[] = [];
  displayedColumns = ['name', 'description', 'actions'];
  showForm = false;
  editingId: string | null = null;

  form = this.fb.group({
    name: ['', Validators.required],
    description: ['']
  });

  ngOnInit() {
    this.loadCategories();
  }

  loadCategories() {
    this.categoryService.getCategories().subscribe(categories => this.categories = categories);
  }

  openCreate() {
    this.form.reset();
    this.editingId = null;
    this.showForm = true;
  }

  openEdit(category: Category) {
    this.form.patchValue(category);
    this.editingId = category.id;
    this.showForm = true;
  }

  cancel() {
    this.showForm = false;
    this.editingId = null;
  }

  save() {
    if (this.form.invalid) return;
    const request = this.form.getRawValue() as any;

    if (this.editingId) {
      this.categoryService.updateCategory(this.editingId, request).subscribe(() => {
        this.snackBar.open('Category updated!', '', { duration: 2000 });
        this.showForm = false;
        this.editingId = null;
        this.loadCategories();
      });
    } else {
      this.categoryService.createCategory(request).subscribe(() => {
        this.snackBar.open('Category created!', '', { duration: 2000 });
        this.showForm = false;
        this.loadCategories();
      });
    }
  }

  deleteCategory(id: string) {
    if (confirm('Delete this category?')) {
      this.categoryService.deleteCategory(id).subscribe(() => {
        this.snackBar.open('Category deleted.', '', { duration: 2000 });
        this.loadCategories();
      });
    }
  }
}
