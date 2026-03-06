import { Injectable, inject } from '@angular/core';
import { Router } from '@angular/router';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { Store } from '@ngrx/store';
import { of } from 'rxjs';
import { map, mergeMap, catchError, tap, withLatestFrom } from 'rxjs/operators';
import { BasketService } from '../../core/services';
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
}
