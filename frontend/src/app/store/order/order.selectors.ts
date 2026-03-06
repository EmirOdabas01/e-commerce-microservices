import { createFeatureSelector, createSelector } from '@ngrx/store';
import { OrderState, orderAdapter } from './order.reducer';

export const selectOrderState = createFeatureSelector<OrderState>('order');

const { selectAll } = orderAdapter.getSelectors();

export const selectAllOrders = createSelector(selectOrderState, selectAll);
export const selectOrderLoading = createSelector(selectOrderState, state => state.loading);
export const selectOrderTotalCount = createSelector(selectOrderState, state => state.totalCount);
