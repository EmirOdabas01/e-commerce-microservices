import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { map, mergeMap, catchError, tap, withLatestFrom, switchMap } from 'rxjs/operators';
import { BasketService } from '../../core/services';
import { AuthActions } from '../auth/auth.actions';
import { CartActions } from './cart.actions';
import { selectCartState } from './cart.selectors';

@Injectable()
export class CartEffects {
  private actions$ = inject(Actions);
  private basketService = inject(BasketService);
  private store = inject(Store);
  private router = inject(Router);

  loadCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.loadCart),
      mergeMap(({ userName }) =>
        this.basketService.getBasket(userName).pipe(
          map(cart => CartActions.loadCartSuccess({ cart })),
          catchError(error => of(CartActions.loadCartFailure({ error: error.message })))
        )
      )
    )
  );

  syncCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.addToCart, CartActions.removeFromCart, CartActions.updateQuantity),
      withLatestFrom(this.store.select(selectCartState)),
      mergeMap(([, state]) =>
        this.basketService.storeBasket({
          userName: state.userName,
          items: state.items,
          totalPrice: state.totalPrice
        }).pipe(
          map(cart => CartActions.storeCartSuccess({ cart })),
          catchError(error => of(CartActions.storeCartFailure({ error: error.message })))
        )
      )
    )
  );

  checkout$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.checkout),
      mergeMap(({ checkout }) =>
        this.basketService.checkout(checkout).pipe(
          map(() => CartActions.checkoutSuccess()),
          catchError(error => of(CartActions.checkoutFailure({ error: error.message })))
        )
      )
    )
  );

  checkoutSuccess$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.checkoutSuccess),
      tap(() => this.router.navigate(['/profile']))
    ),
    { dispatch: false }
  );

  persistGuestCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.addToCart, CartActions.removeFromCart, CartActions.updateQuantity),
      withLatestFrom(this.store.select(selectCartState)),
      tap(([, state]) => {
        if (state.userName === 'guest') {
          localStorage.setItem('guest_cart', JSON.stringify({
            userName: state.userName,
            items: state.items,
            totalPrice: state.totalPrice
          }));
        }
      })
    ),
    { dispatch: false }
  );

  clearGuestCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(CartActions.clearCart, CartActions.checkoutSuccess),
      tap(() => localStorage.removeItem('guest_cart'))
    ),
    { dispatch: false }
  );

  transferGuestCart$ = createEffect(() =>
    this.actions$.pipe(
      ofType(AuthActions.loadUserSuccess),
      withLatestFrom(this.store.select(selectCartState)),
      switchMap(([{ user }, cartState]) => {
        if (cartState.userName !== 'guest' || cartState.items.length === 0) {
          return of(CartActions.loadCart({ userName: user.userName }));
        }
        return this.basketService.storeBasket({
          userName: user.userName,
          items: cartState.items,
          totalPrice: cartState.totalPrice
        }).pipe(
          switchMap(() => this.basketService.deleteBasket('guest')),
          tap(() => localStorage.removeItem('guest_cart')),
          map(() => CartActions.loadCart({ userName: user.userName })),
          catchError(() => of(CartActions.loadCart({ userName: user.userName })))
        );
      })
    )
  );
}
