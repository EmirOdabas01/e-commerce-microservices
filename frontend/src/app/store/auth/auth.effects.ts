import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap, delay } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { AuthService, CookieService } from '../../core/services';
import { AuthActions } from './auth.actions';

@Injectable()
export class AuthEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);
  private cookieService = inject(CookieService);
  private snackBar = inject(MatSnackBar);
  private router = inject(Router);

  login$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.login),
      exhaustMap(({ request }) =>
        this.authService.login(request).pipe(
          map(response => AuthActions.loginSuccess({ response })),
          catchError(error => of(AuthActions.loginFailure({ error: error.error?.message || 'Login failed' })))
        )
      )
    )
  );

  register$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.register),
      exhaustMap(({ request }) =>
        this.authService.register(request).pipe(
          map(response => AuthActions.registerSuccess({ response })),
          catchError(error => {
            let message = 'Registration failed';
            if (Array.isArray(error.error)) {
              message = error.error.map((e: any) => e.description).join('. ');
            } else if (error.error?.message) {
              message = error.error.message;
            }
            return of(AuthActions.registerFailure({ error: message }));
          })
        )
      )
    )
  );

  authSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginSuccess, AuthActions.registerSuccess),
      tap(({ response }) => {
        this.cookieService.set('token', response.token);
        this.cookieService.set('refreshToken', response.refreshToken);
      }),
      map(() => AuthActions.loadUser())
    )
  );

  loadUser$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loadUser),
      exhaustMap(() =>
        this.authService.getMe().pipe(
          map(user => AuthActions.loadUserSuccess({ user })),
          catchError(error => of(AuthActions.loadUserFailure({ error: error.message })))
        )
      )
    )
  );

  loginRedirect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginSuccess),
      tap(() => this.router.navigate(['/']))
    ),
    { dispatch: false }
  );

  registerRedirect$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.registerSuccess),
      tap(() => this.snackBar.open('Registration successful!', '', { duration: 2000 })),
      delay(2000),
      tap(() => this.router.navigate(['/']))
    ),
    { dispatch: false }
  );

  updateProfile$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.updateProfile),
      exhaustMap(({ request }) =>
        this.authService.updateProfile(request).pipe(
          map(user => AuthActions.updateProfileSuccess({ user })),
          catchError(error => of(AuthActions.updateProfileFailure({ error: error.error?.message || 'Update failed' })))
        )
      )
    )
  );

  updateProfileSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.updateProfileSuccess),
      tap(() => this.snackBar.open('Profile updated successfully!', '', { duration: 2000 }))
    ),
    { dispatch: false }
  );

  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      exhaustMap(() =>
        this.authService.logout().pipe(
          catchError(() => of(undefined))
        )
      ),
      tap(() => {
        this.cookieService.delete('token');
        this.cookieService.delete('refreshToken');
        window.location.href = '/';
      })
    ),
    { dispatch: false }
  );
}
