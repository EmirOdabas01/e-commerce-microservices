import { Component, inject, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { Store } from '@ngrx/store';
import { HeaderComponent } from './shared/components/header/header.component';
import { FooterComponent } from './shared/components/footer/footer.component';
import { AuthActions } from './store/auth/auth.actions';
import { CartActions } from './store/cart/cart.actions';

@Component({
  selector: 'app-root',
  imports: [RouterOutlet, HeaderComponent, FooterComponent],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App implements OnInit {
  private store = inject(Store);

  ngOnInit() {
    const token = localStorage.getItem('token');
    const refreshToken = localStorage.getItem('refreshToken');
    if (token && refreshToken) {
      this.store.dispatch(AuthActions.restoreToken({ token, refreshToken }));
      this.store.dispatch(AuthActions.loadUser());
    }
    this.store.dispatch(CartActions.loadCart({ userName: 'guest' }));
  }
}
