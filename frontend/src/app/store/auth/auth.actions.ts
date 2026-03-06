import { createActionGroup, emptyProps, props } from '@ngrx/store';
import { LoginRequest, RegisterRequest, AuthResponse, User } from '../../core/models';

export const AuthActions = createActionGroup({
  source: 'Auth',
  events: {
    'Login': props<{ request: LoginRequest }>(),
    'Login Success': props<{ response: AuthResponse }>(),
    'Login Failure': props<{ error: string }>(),
    'Register': props<{ request: RegisterRequest }>(),
    'Register Success': props<{ response: AuthResponse }>(),
    'Register Failure': props<{ error: string }>(),
    'Load User': emptyProps(),
    'Load User Success': props<{ user: User }>(),
    'Load User Failure': props<{ error: string }>(),
    'Logout': emptyProps(),
    'Restore Token': props<{ token: string; refreshToken: string }>(),
  }
});
