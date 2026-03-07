import { HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { Store } from '@ngrx/store';
import { selectToken } from '../../store/auth/auth.selectors';
import { take, switchMap } from 'rxjs';
import { CookieService } from '../services';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
  const store = inject(Store);
  const cookieService = inject(CookieService);
  return store.select(selectToken).pipe(
    take(1),
    switchMap(storeToken => {
      const token = storeToken || cookieService.get('token');
      if (token) {
        const cloned = req.clone({
          setHeaders: { Authorization: `Bearer ${token}` }
        });
        return next(cloned);
      }
      return next(req);
    })
  );
};
