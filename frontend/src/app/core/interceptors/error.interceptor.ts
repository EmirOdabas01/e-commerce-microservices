import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Router } from '@angular/router';
import { MatSnackBar } from '@angular/material/snack-bar';
import { catchError, throwError } from 'rxjs';

export const errorInterceptor: HttpInterceptorFn = (req, next) => {
  const router = inject(Router);
  const snackBar = inject(MatSnackBar);

  return next(req).pipe(
    catchError(error => {
      if (error.status === 401) {
        router.navigate(['/auth/login']);
      } else if (error.status === 0) {
        snackBar.open('Server is not reachable', 'Close', { duration: 5000 });
      } else {
        const message = error.error?.message || error.message || 'An error occurred';
        snackBar.open(message, 'Close', { duration: 5000 });
      }
      return throwError(() => error);
    })
  );
};
