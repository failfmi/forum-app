import { Component, OnInit } from '@angular/core';
import { AppState } from '../../../core/store/app.state';
import { Store, select } from '@ngrx/store';
import { AdminService } from '../../../core/services/admin/admin.service';
import { BaseComponent } from '../../base.component';
import { UserModel } from '../../../core/models/users/users.model';
import { CategoryEditModel } from '../../../core/models/category/categoryEdit.model';
import { CategoriesService } from '../../../core/services/categories/categories.service';

@Component({
  selector: 'app-admin-panel',
  templateUrl: './admin-panel.component.html',
  styleUrls: ['./admin-panel.component.scss']
})
export class AdminPanelComponent extends BaseComponent {
  private subscription$;
  protected users: UserModel[] = [];
  protected categories: CategoryEditModel[] = [];
  protected showCategory: boolean;
  protected showUser: boolean;

  constructor(private store: Store<AppState>, private adminService: AdminService, private categoryService: CategoriesService) {
    super();
    this.adminService.getAllUsers();
    this.categoryService.getAllCategories();
    this.subscription$ = this.store
      .pipe(select(st => st))
      .subscribe(st => {
        this.users = st.users.all;
        this.users.forEach(u => u.dateRegistered = new Date(u.dateRegistered));
        this.categories = st.categories.all;
      });

    this.subscriptions.push(this.subscription$);
  }

  showCategories() {
    this.showUser = false;
    this.showCategory = true;
  }

  showUsers() {
    this.showCategory = false;
    this.showUser = true;
  }
}
