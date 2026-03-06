import { createFeatureSelector, createSelector } from '@ngrx/store';
import { ProductState, productAdapter } from './product.reducer';

export const selectProductState = createFeatureSelector<ProductState>('product');

const { selectAll, selectEntities } = productAdapter.getSelectors();

export const selectAllProducts = createSelector(selectProductState, selectAll);
export const selectProductEntities = createSelector(selectProductState, selectEntities);
export const selectSelectedProduct = createSelector(selectProductState, state => state.selectedProduct);
export const selectProductLoading = createSelector(selectProductState, state => state.loading);
export const selectProductTotalCount = createSelector(selectProductState, state => state.totalCount);
export const selectProductPageIndex = createSelector(selectProductState, state => state.pageIndex);
export const selectProductPageSize = createSelector(selectProductState, state => state.pageSize);
