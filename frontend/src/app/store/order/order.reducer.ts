import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { OrderDto } from '../../core/models';
import { OrderActions } from './order.actions';

export interface OrderState extends EntityState<OrderDto> {
  totalCount: number;
  loading: boolean;
  error: string | null;
}

export const orderAdapter: EntityAdapter<OrderDto> = createEntityAdapter<OrderDto>();

const initialState: OrderState = orderAdapter.getInitialState({
  totalCount: 0,
  loading: false,
  error: null
});

export const orderReducer = createReducer(
  initialState,
  on(OrderActions.loadOrders, OrderActions.loadUserOrders, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(OrderActions.loadOrdersSuccess, (state, { result }) =>
    orderAdapter.setAll(result.data, {
      ...state,
      totalCount: result.count,
      loading: false
    })
  ),
  on(OrderActions.loadUserOrdersSuccess, (state, { orders }) =>
    orderAdapter.setAll(orders, {
      ...state,
      totalCount: orders.length,
      loading: false
    })
  ),
  on(OrderActions.loadOrdersFailure, OrderActions.loadUserOrdersFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);
