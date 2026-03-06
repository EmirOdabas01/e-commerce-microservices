import { Injectable, inject } from '@angular/core';
import { Actions, createEffect, ofType } from '@ngrx/effects';
import { of } from 'rxjs';
import { map, mergeMap, catchError } from 'rxjs/operators';
import { OrderService } from '../../core/services';
import { OrderActions } from './order.actions';

@Injectable()
export class OrderEffects {
  private actions$ = inject(Actions);
  private orderService = inject(OrderService);

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
}
