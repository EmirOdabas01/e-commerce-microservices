export interface PaymentMethodResponse {
  id: string;
  label: string;
  cardName: string;
  cardNumberLast4: string;
  expiration: string;
  isDefault: boolean;
}

export interface PaymentMethodRequest {
  label: string;
  cardName: string;
  cardNumber: string;
  expiration: string;
  isDefault: boolean;
}
