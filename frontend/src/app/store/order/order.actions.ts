import { createActionGroup, props } from '@ngrx/store';
import { OrderDto, PaginatedResult } from '../../core/models';

export const OrderActions = createActionGroup({
  source: 'Order',
  events: {
    'Load Orders': props<{ pageIndex: number; pageSize: number }>(),
    'Load Orders Success': props<{ result: PaginatedResult<OrderDto> }>(),
    'Load Orders Failure': props<{ error: string }>(),
    'Load User Orders': props<{ userName: string }>(),
    'Load User Orders Success': props<{ orders: OrderDto[] }>(),
    'Load User Orders Failure': props<{ error: string }>(),
    'Cancel Order': props<{ id: string; userName: string }>(),
    'Cancel Order Success': props<{ id: string; userName: string }>(),
    'Cancel Order Failure': props<{ error: string }>(),
    'Refund Order': props<{ id: string; userName: string }>(),
    'Refund Order Success': props<{ id: string; userName: string }>(),
    'Refund Order Failure': props<{ error: string }>(),
  }
});
