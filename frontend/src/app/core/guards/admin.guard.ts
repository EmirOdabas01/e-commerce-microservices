import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { Store } from '@ngrx/store';
import { selectIsAdmin } from '../../store/auth/auth.selectors';
import { map, take } from 'rxjs';

export const adminGuard: CanActivateFn = () => {
  const store = inject(Store);
  const router = inject(Router);

  return store.select(selectIsAdmin).pipe(
    take(1),
    map(isAdmin => {
      if (!isAdmin) {
        router.navigate(['/']);
        return false;
      }
      return true;
    })
  );
};
