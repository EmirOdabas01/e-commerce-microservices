import { Component, inject } from '@angular/core';
import { RouterLink, RouterLinkActive } from '@angular/router';
import { Store } from '@ngrx/store';
import { AsyncPipe } from '@angular/common';
import { MatToolbarModule } from '@angular/material/toolbar';
import { MatButtonModule } from '@angular/material/button';
import { MatIconModule } from '@angular/material/icon';
import { MatBadgeModule } from '@angular/material/badge';
import { MatMenuModule } from '@angular/material/menu';
import { selectIsAuthenticated, selectUser, selectIsAdmin } from '../../../store/auth/auth.selectors';
import { selectCartItemCount } from '../../../store/cart/cart.selectors';
import { AuthActions } from '../../../store/auth/auth.actions';

@Component({
  selector: 'app-header',
  imports: [
    RouterLink,
    RouterLinkActive,
    AsyncPipe,
    MatToolbarModule,
    MatButtonModule,
    MatIconModule,
    MatBadgeModule,
    MatMenuModule
  ],
  templateUrl: './header.component.html',
  styleUrl: './header.component.scss'
})
export class HeaderComponent {
  private store = inject(Store);

  isAuthenticated$ = this.store.select(selectIsAuthenticated);
  user$ = this.store.select(selectUser);
  isAdmin$ = this.store.select(selectIsAdmin);
  cartItemCount$ = this.store.select(selectCartItemCount);

  logout() {
    this.store.dispatch(AuthActions.logout());
  }
}
