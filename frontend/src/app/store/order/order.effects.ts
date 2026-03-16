import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError, tap } from 'rxjs/operators';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrderService } from '../../core/services';
import { OrderActions } from './order.actions';

@Injectable()
export class OrderEffects {
  private actions$ = inject(Actions);
  private orderService = inject(OrderService);
  private snackBar = inject(MatSnackBar);

  loadOrders$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.loadOrders),
      mergeMap(({ pageIndex, pageSize }) =>
        this.orderService.getOrders(pageIndex, pageSize).pipe(
          map(result => OrderActions.loadOrdersSuccess({ result })),
          catchError(error => of(OrderActions.loadOrdersFailure({ error: error.message })))
        )
      )
    )
  );

  loadUserOrders$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.loadUserOrders),
      mergeMap(({ userName }) =>
        this.orderService.getOrdersByUser(userName).pipe(
          map(orders => OrderActions.loadUserOrdersSuccess({ orders })),
          catchError(error => of(OrderActions.loadUserOrdersFailure({ error: error.message })))
        )
      )
    )
  );

  cancelOrder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.cancelOrder),
      mergeMap(({ id, userName }) =>
        this.orderService.cancelOrder(id).pipe(
          tap(() => this.snackBar.open('Order cancelled.', '', { duration: 2000 })),
          map(() => OrderActions.cancelOrderSuccess({ id, userName })),
          catchError(error => of(OrderActions.cancelOrderFailure({ error: error.error?.message || error.message })))
        )
      )
    )
  );

  refundOrder$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.refundOrder),
      mergeMap(({ id, userName }) =>
        this.orderService.refundOrder(id).pipe(
          tap(() => this.snackBar.open('Refund requested.', '', { duration: 2000 })),
          map(() => OrderActions.refundOrderSuccess({ id, userName })),
          catchError(error => of(OrderActions.refundOrderFailure({ error: error.error?.message || error.message })))
        )
      )
    )
  );

  reloadAfterAction$ = createEffect(() =>
    this.actions$.pipe(
      ofType(OrderActions.cancelOrderSuccess, OrderActions.refundOrderSuccess),
      map(({ userName }) => OrderActions.loadUserOrders({ userName }))
    )
  );
}
