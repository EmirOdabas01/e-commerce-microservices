import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, SlicePipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatChipsModule } from '@angular/material/chips';
import { selectUser } from '../../store/auth/auth.selectors';
import { selectAllOrders, selectOrderLoading } from '../../store/order/order.selectors';
import { OrderActions } from '../../store/order/order.actions';
import { OrderStatus } from '../../core/models';
import { LoadingSpinnerComponent } from '../../shared/components/loading-spinner/loading-spinner.component';
import { take } from 'rxjs';

@Component({
  selector: 'app-profile',
  imports: [AsyncPipe, CurrencyPipe, SlicePipe, MatCardModule, MatTableModule, MatChipsModule, LoadingSpinnerComponent],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private store = inject(Store);

  user$ = this.store.select(selectUser);
  orders$ = this.store.select(selectAllOrders);
  loading$ = this.store.select(selectOrderLoading);
  displayedColumns = ['id', 'status', 'totalPrice'];
  OrderStatus = OrderStatus;

  getStatusLabel(status: OrderStatus): string {
    return OrderStatus[status] ?? 'Unknown';
  }

  ngOnInit() {
    this.store.select(selectUser).pipe(take(1)).subscribe(user => {
      if (user) {
        this.store.dispatch(OrderActions.loadUserOrders({ userName: user.userName }));
      }
    });
  }
}
