import { Component, inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { CurrencyPipe } from '@angular/common';
import { MatCardModule } from '@angular/material/card';
import { MatTableModule } from '@angular/material/table';
import { MatIconModule } from '@angular/material/icon';
import { environment } from '../../../../environments/environment';

interface Analytics {
  totalOrders: number;
  totalRevenue: number;
  averageOrderValue: number;
  pendingOrders: number;
  cancelledOrders: number;
  topProducts: { productName: string; totalQuantity: number; totalRevenue: number }[];
  statusBreakdown: { status: string; count: number }[];
}

interface LowStockProduct {
  id: string;
  name: string;
  stock: number;
}

@Component({
  selector: 'app-admin-dashboard',
  imports: [CurrencyPipe, MatCardModule, MatTableModule, MatIconModule],
  templateUrl: './admin-dashboard.component.html',
  styleUrl: './admin-dashboard.component.scss'
})
export class AdminDashboardComponent implements OnInit {
  private http = inject(HttpClient);

  analytics: Analytics | null = null;
  lowStockProducts: LowStockProduct[] = [];
  topProductColumns = ['productName', 'totalQuantity', 'totalRevenue'];
  lowStockColumns = ['name', 'stock'];

  ngOnInit() {
    this.http.get<Analytics>(`${environment.apiUrl}/api/orders/analytics`).subscribe(data => {
      this.analytics = data;
    });
    this.http.get<LowStockProduct[]>(`${environment.apiUrl}/api/products/low-stock`).subscribe(data => {
      this.lowStockProducts = data;
    });
  }
}
