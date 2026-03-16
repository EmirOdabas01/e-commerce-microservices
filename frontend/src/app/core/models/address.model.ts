export interface Address {
  id: string;
  label: string;
  firstName: string;
  lastName: string;
  addressLine: string;
  country: string;
  state: string;
  zipCode: string;
  emailAddress: string;
  isDefault: boolean;
}

export interface AddressRequest {
  label: string;
  firstName: string;
  lastName: string;
  addressLine: string;
  country: string;
  state: string;
  zipCode: string;
  emailAddress: string;
  isDefault: boolean;
}
