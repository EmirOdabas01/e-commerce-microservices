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

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  userName: string;
  roles: string[];
}
