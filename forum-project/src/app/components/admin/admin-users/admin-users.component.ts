import { Component, OnInit, Input } from '@angular/core';
import { AdminService } from '../../../core/services/admin/admin.service';
import { adminCategoriesAnimations } from '../admin-categories/admin-categories.animations';

@Component({
  selector: 'app-admin-users',
  templateUrl: './admin-users.component.html',
  styleUrls: ['./admin-users.component.scss'],
  animations: adminCategoriesAnimations
})
export class AdminUsersComponent {
  @Input() users;
  protected pageSize = 6;
  protected currentPage = 1;

  constructor(private adminService: AdminService) { }

  banUser(userId: string) {
    this.adminService.banUser(userId);
  }

  unbanUser(userId: string) {
    this.adminService.unbanUser(userId);
  }
}
