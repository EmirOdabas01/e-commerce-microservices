import { Component, inject, OnInit } from '@angular/core';
import { Store } from '@ngrx/store';
import { AsyncPipe, CurrencyPipe, SlicePipe } from '@angular/common';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatSelectModule } from '@angular/material/select';
import { FormsModule } from '@angular/forms';
import { MatSnackBar } from '@angular/material/snack-bar';
import { OrderActions } from '../../../store/order/order.actions';
import { selectAllOrders, selectOrderLoading } from '../../../store/order/order.selectors';
import { OrderStatus } from '../../../core/models';
import { OrderService } from '../../../core/services';
import { LoadingSpinnerComponent } from '../../../shared/components/loading-spinner/loading-spinner.component';

@Component({
  selector: 'app-admin-orders',
  imports: [
    AsyncPipe, CurrencyPipe, SlicePipe, FormsModule,
    MatTableModule, MatButtonModule, MatChipsModule, MatSelectModule,
    LoadingSpinnerComponent
  ],
  templateUrl: './admin-orders.component.html',
  styleUrl: './admin-orders.component.scss'
})
export class AdminOrdersComponent implements OnInit {
  private store = inject(Store);
  private orderService = inject(OrderService);
  private snackBar = inject(MatSnackBar);

  orders$ = this.store.select(selectAllOrders);
  loading$ = this.store.select(selectOrderLoading);
  displayedColumns = ['id', 'userName', 'status', 'totalPrice', 'actions'];
  OrderStatus = OrderStatus;
  statuses = [
    { value: OrderStatus.Pending, label: 'Pending' },
    { value: OrderStatus.Processing, label: 'Processing' },
    { value: OrderStatus.Completed, label: 'Completed' },
    { value: OrderStatus.Cancelled, label: 'Cancelled' },
    { value: OrderStatus.Failed, label: 'Failed' },
    { value: OrderStatus.Refunded, label: 'Refunded' }
  ];

  getStatusLabel(status: OrderStatus): string {
    return OrderStatus[status] ?? 'Unknown';
  }

  ngOnInit() {
    this.store.dispatch(OrderActions.loadOrders({ pageIndex: 0, pageSize: 100 }));
  }

  cancelOrder(id: string) {
    this.orderService.cancelOrder(id).subscribe(() => {
      this.snackBar.open('Order cancelled.', '', { duration: 2000 });
      this.store.dispatch(OrderActions.loadOrders({ pageIndex: 0, pageSize: 100 }));
    });
  }

  refundOrder(id: string) {
    this.orderService.refundOrder(id).subscribe(() => {
      this.snackBar.open('Refund processed.', '', { duration: 2000 });
      this.store.dispatch(OrderActions.loadOrders({ pageIndex: 0, pageSize: 100 }));
    });
  }
}
