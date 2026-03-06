import { createReducer, on } from '@ngrx/store';
import { User } from '../../core/models';
import { AuthActions } from './auth.actions';

export interface AuthState {
  user: User | null;
  token: string | null;
  refreshToken: string | null;
  isAuthenticated: boolean;
  loading: boolean;
  error: string | null;
}

const initialState: AuthState = {
  user: null,
  token: null,
  refreshToken: null,
  isAuthenticated: false,
  loading: false,
  error: null
};

export const authReducer = createReducer(
  initialState,
  on(AuthActions.login, AuthActions.register, (state) => ({
    ...state,
    loading: true,
    error: null
  })),
  on(AuthActions.loginSuccess, AuthActions.registerSuccess, (state, { response }) => ({
    ...state,
    token: response.token,
    refreshToken: response.refreshToken,
    isAuthenticated: true,
    loading: false,
    error: null
  })),
  on(AuthActions.loginFailure, AuthActions.registerFailure, (state, { error }) => ({
    ...state,
    loading: false,
    error
  })),
  on(AuthActions.loadUserSuccess, (state, { user }) => ({
    ...state,
    user
  })),
  on(AuthActions.logout, () => initialState),
  on(AuthActions.restoreToken, (state, { token, refreshToken }) => ({
    ...state,
    token,
    refreshToken,
    isAuthenticated: true
  }))
);
