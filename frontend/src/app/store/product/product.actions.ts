import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { Product, PaginatedResult } from '../../core/models';

export const ProductActions = createActionGroup({
  source: 'Product',
  events: {
    'Load Products': props<{ pageIndex: number; pageSize: number }>(),
    'Load Products Success': props<{ result: PaginatedResult<Product> }>(),
    'Load Products Failure': props<{ error: string }>(),
    'Load Product': props<{ id: string }>(),
    'Load Product Success': props<{ product: Product }>(),
    'Load Product Failure': props<{ error: string }>(),
    'Load Products By Category': props<{ category: string }>(),
    'Load Products By Category Success': props<{ products: Product[] }>(),
    'Load Products By Category Failure': props<{ error: string }>(),
    'Create Product': props<{ product: Product }>(),
    'Create Product Success': props<{ product: Product }>(),
    'Create Product Failure': props<{ error: string }>(),
    'Update Product': props<{ product: Product }>(),
    'Update Product Success': props<{ product: Product }>(),
    'Update Product Failure': props<{ error: string }>(),
    'Delete Product': props<{ id: string }>(),
    'Delete Product Success': props<{ id: string }>(),
    'Delete Product Failure': props<{ error: string }>(),
    'Search Products': props<{ query: string; pageIndex: number; pageSize: number }>(),
    'Search Products Success': props<{ result: PaginatedResult<Product> }>(),
    'Search Products Failure': props<{ error: string }>(),
  }
});
