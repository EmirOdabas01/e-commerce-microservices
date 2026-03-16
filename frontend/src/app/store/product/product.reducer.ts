import { createReducer, on } from '@ngrx/store';
import { EntityState, EntityAdapter, createEntityAdapter } from '@ngrx/entity';
import { Product } from '../../core/models';
import { ProductActions } from './product.actions';

export interface ProductState extends EntityState<Product> {
  selectedProduct: Product | null;
  totalCount: number;
  pageIndex: number;
  pageSize: number;
  loading: boolean;
  error: string | null;
}

export const productAdapter: EntityAdapter<Product> = createEntityAdapter<Product>();

const initialState: ProductState = productAdapter.getInitialState({
  selectedProduct: null,
  totalCount: 0,
  pageIndex: 0,
  pageSize: 10,
  loading: false,
  error: null
});

export const productReducer = createReducer(
  initialState,
  on(ProductActions.loadProducts, ProductActions.loadProductsByCategory, ProductActions.searchProducts, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(ProductActions.loadProductsSuccess, (state, { result }) =>
    productAdapter.setAll(result.data, {
      ...state,
      totalCount: result.count,
      pageIndex: result.pageIndex,
      pageSize: result.pageSize,
      loading: false
    })
  ),
  on(ProductActions.searchProductsSuccess, (state, { result }) =>
    productAdapter.setAll(result.data, {
      ...state,
      totalCount: result.count,
      pageIndex: result.pageIndex,
      pageSize: result.pageSize,
      loading: false
    })
  ),
  on(ProductActions.loadProductsByCategorySuccess, (state, { products }) =>
    productAdapter.setAll(products, {
      ...state,
      totalCount: products.length,
      loading: false
    })
  ),
  on(ProductActions.loadProduct, (state) => ({
    ...state,
    loading: true
  })),
  on(ProductActions.loadProductSuccess, (state, { product }) => ({
    ...state,
    selectedProduct: product,
    loading: false
  })),
  on(ProductActions.createProductSuccess, (state, { product }) =>
    productAdapter.addOne(product, { ...state, loading: false })
  ),
  on(ProductActions.updateProductSuccess, (state, { product }) =>
    productAdapter.updateOne({ id: product.id, changes: product }, { ...state, loading: false })
  ),
  on(ProductActions.deleteProductSuccess, (state, { id }) =>
    productAdapter.removeOne(id, { ...state, loading: false })
  ),
  on(
    ProductActions.loadProductsFailure,
    ProductActions.loadProductFailure,
    ProductActions.loadProductsByCategoryFailure,
    ProductActions.createProductFailure,
    ProductActions.updateProductFailure,
    ProductActions.deleteProductFailure,
    ProductActions.searchProductsFailure,
    (state, { error }) => ({ ...state, loading: false, error })
  )
);
