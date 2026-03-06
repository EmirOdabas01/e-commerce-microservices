export interface ShoppingCartItem {
  quantity: number;
  price: number;
  productId: string;
  productName: string;
}

export interface ShoppingCart {
  userName: string;
  items: ShoppingCartItem[];
  totalPrice: number;
}

export interface BasketCheckout {
  userName: string;
  customerId: string;
  totalPrice: number;
  firstName: string;
  lastName: string;
  emailAddress: string;
  addressLine: string;
  country: string;
  state: string;
  zipCode: string;
  cardName: string;
  cardNumber: string;
  expiration: string;
  cvv: string;
  paymentMethod: number;
}
