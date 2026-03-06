import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { ShoppingCart, ShoppingCartItem, BasketCheckout } from '../../core/models';

export const CartActions = createActionGroup({
  source: 'Cart',
  events: {
    'Load Cart': props<{ userName: string }>(),
    'Load Cart Success': props<{ cart: ShoppingCart }>(),
    'Load Cart Failure': props<{ error: string }>(),
    'Store Cart': props<{ cart: ShoppingCart }>(),
    'Store Cart Success': props<{ cart: ShoppingCart }>(),
    'Store Cart Failure': props<{ error: string }>(),
    'Add To Cart': props<{ item: ShoppingCartItem }>(),
    'Remove From Cart': props<{ productId: string }>(),
    'Update Quantity': props<{ productId: string; quantity: number }>(),
    'Clear Cart': emptyProps(),
    'Checkout': props<{ checkout: BasketCheckout }>(),
    'Checkout Success': emptyProps(),
    'Checkout Failure': props<{ error: string }>(),
  }
});
