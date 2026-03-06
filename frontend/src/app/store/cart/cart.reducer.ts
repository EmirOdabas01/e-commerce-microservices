import { createReducer, on } from '@ngrx/store';
import { ShoppingCartItem } from '../../core/models';
import { CartActions } from './cart.actions';

export interface CartState {
  userName: string;
  items: ShoppingCartItem[];
  totalPrice: number;
  loading: boolean;
  error: string | null;
}

const initialState: CartState = {
  userName: 'guest',
  items: [],
  totalPrice: 0,
  loading: false,
  error: null
};

function calculateTotal(items: ShoppingCartItem[]): number {
  return items.reduce((sum, item) => sum + item.price * item.quantity, 0);
}

export const cartReducer = createReducer(
  initialState,
  on(CartActions.loadCart, (state) => ({ ...state, loading: true })),
  on(CartActions.loadCartSuccess, CartActions.storeCartSuccess, (state, { cart }) => ({
    ...state,
    userName: cart.userName,
    items: cart.items,
    totalPrice: cart.totalPrice,
    loading: false
  })),
  on(CartActions.addToCart, (state, { item }) => {
    const existing = state.items.find(i => i.productId === item.productId);
    const items = existing
      ? state.items.map(i => i.productId === item.productId ? { ...i, quantity: i.quantity + item.quantity } : i)
      : [...state.items, item];
    return { ...state, items, totalPrice: calculateTotal(items) };
  }),
  on(CartActions.removeFromCart, (state, { productId }) => {
    const items = state.items.filter(i => i.productId !== productId);
    return { ...state, items, totalPrice: calculateTotal(items) };
  }),
  on(CartActions.updateQuantity, (state, { productId, quantity }) => {
    const items = state.items.map(i => i.productId === productId ? { ...i, quantity } : i);
    return { ...state, items, totalPrice: calculateTotal(items) };
  }),
  on(CartActions.clearCart, () => initialState),
  on(CartActions.checkout, (state) => ({ ...state, loading: true })),
  on(CartActions.checkoutSuccess, () => initialState),
  on(CartActions.loadCartFailure, CartActions.storeCartFailure, CartActions.checkoutFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  }))
);
