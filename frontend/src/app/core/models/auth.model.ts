export interface LoginRequest {
  email: string;
  password: string;
}

export interface RegisterRequest {
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  password: string;
  confirmPassword: string;
  role: string;
}

export interface RefreshRequest {
  refreshToken: string;
}

export interface AuthResponse {
  token: string;
  refreshToken: string;
  expiration: string;
}

export interface ForgotPasswordRequest {
  email: string;
}

export interface ResetPasswordRequest {
  email: string;
  token: string;
  newPassword: string;
  confirmPassword: string;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  email: string;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  roles: string[];
}
