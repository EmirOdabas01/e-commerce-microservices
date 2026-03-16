export interface Coupon {
  id: string;
  code: string;
  discountPercent: number;
  maxDiscountAmount: number | null;
  minOrderAmount: number;
  usageLimit: number;
  usedCount: number;
  expiresAt: string;
  isActive: boolean;
}

export interface CouponRequest {
  code: string;
  discountPercent: number;
  maxDiscountAmount: number | null;
  minOrderAmount: number;
  usageLimit: number;
  expiresAt: string;
}

export interface ValidateCouponResponse {
  isValid: boolean;
  discountAmount: number;
  message: string | null;
}
