import { Component, inject } from '@angular/core';
import { RouterLink } from '@angular/router';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatCardModule } from '@angular/material/card';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService } from '../../../core/services';

@Component({
  selector: 'app-forgot-password',
  imports: [RouterLink, ReactiveFormsModule, MatCardModule, MatFormFieldModule, MatInputModule, MatButtonModule],
  templateUrl: './forgot-password.component.html',
  styleUrl: './forgot-password.component.scss'
})
export class ForgotPasswordComponent {
  private fb = inject(FormBuilder);
  private authService = inject(AuthService);
  private snackBar = inject(MatSnackBar);

  loading = false;
  submitted = false;

  form = this.fb.group({
    email: ['', [Validators.required, Validators.email]]
  });

  onSubmit() {
    if (this.form.valid) {
      this.loading = true;
      this.authService.forgotPassword(this.form.getRawValue() as any).subscribe({
        next: () => {
          this.loading = false;
          this.submitted = true;
          this.snackBar.open('If the email exists, a reset link has been sent.', '', { duration: 3000 });
        },
        error: () => {
          this.loading = false;
          this.snackBar.open('Something went wrong.', '', { duration: 3000 });
        }
      });
    }
  }
}
