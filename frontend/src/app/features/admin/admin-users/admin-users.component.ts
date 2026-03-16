import { Component, inject, OnInit } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { MatTableModule } from '@angular/material/table';
import { MatButtonModule } from '@angular/material/button';
import { MatChipsModule } from '@angular/material/chips';
import { MatSnackBar } from '@angular/material/snack-bar';
import { environment } from '../../../../environments/environment';

interface AdminUser {
  id: string;
  email: string;
  firstName: string;
  lastName: string;
  userName: string;
  isLocked: boolean;
  roles: string[];
}

@Component({
  selector: 'app-admin-users',
  imports: [MatTableModule, MatButtonModule, MatChipsModule],
  templateUrl: './admin-users.component.html',
  styleUrl: './admin-users.component.scss'
})
export class AdminUsersComponent implements OnInit {
  private http = inject(HttpClient);
  private snackBar = inject(MatSnackBar);

  users: AdminUser[] = [];
  displayedColumns = ['name', 'email', 'roles', 'status', 'actions'];

  ngOnInit() {
    this.loadUsers();
  }

  loadUsers() {
    this.http.get<AdminUser[]>(`${environment.apiUrl}/api/identity/admin/users`).subscribe(users => {
      this.users = users;
    });
  }

  suspend(id: string) {
    this.http.put(`${environment.apiUrl}/api/identity/admin/users/${id}/suspend`, {}).subscribe(() => {
      this.snackBar.open('User suspended.', '', { duration: 2000 });
      this.loadUsers();
    });
  }

  unsuspend(id: string) {
    this.http.put(`${environment.apiUrl}/api/identity/admin/users/${id}/unsuspend`, {}).subscribe(() => {
      this.snackBar.open('User unsuspended.', '', { duration: 2000 });
      this.loadUsers();
    });
  }
}
