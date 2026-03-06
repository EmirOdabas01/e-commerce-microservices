import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, exhaustMap, catchError, tap } from 'rxjs/operators';
import { AuthService } from '../../core/services';
import { AuthActions } from './auth.actions';

@Injectable()
export class AuthEffects {
  private actions$ = inject(Actions);
  private authService = inject(AuthService);
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
          catchError(error => of(AuthActions.registerFailure({ error: error.error?.message || 'Registration failed' })))
        )
      )
    )
  );

  authSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loginSuccess, AuthActions.registerSuccess),
      tap(({ response }) => {
        localStorage.setItem('token', response.token);
        localStorage.setItem('refreshToken', response.refreshToken);
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

  loadUserSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loadUserSuccess),
      tap(() => this.router.navigate(['/']))
    ),
    { dispatch: false }
  );

  logout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.logout),
      tap(() => {
        localStorage.removeItem('token');
        localStorage.removeItem('refreshToken');
        this.router.navigate(['/']);
      })
    ),
    { dispatch: false }
  );
}
