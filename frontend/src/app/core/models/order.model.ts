export interface OrderItemDto {
  id: string;
  productId: string;
  productName: string;
  quantity: number;
  price: number;
}

export interface OrderDto {
  id: string;
  userName: string;
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
  status: OrderStatus;
  items: OrderItemDto[];
}

export enum OrderStatus {
  Pending = 1,
  Processing = 2,
  Completed = 3,
  Cancelled = 4,
  Failed = 5
}
