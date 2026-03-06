import { createFeatureSelector, createSelector } from '@ngrx/store';
import { CartState } from './cart.reducer';

export const selectCartState = createFeatureSelector<CartState>('cart');

export const selectCartItems = createSelector(selectCartState, state => state.items);
export const selectCartTotalPrice = createSelector(selectCartState, state => state.totalPrice);
export const selectCartItemCount = createSelector(selectCartItems, items => items.reduce((sum, i) => sum + i.quantity, 0));
export const selectCartLoading = createSelector(selectCartState, state => state.loading);
export const selectCartUserName = createSelector(selectCartState, state => state.userName);
