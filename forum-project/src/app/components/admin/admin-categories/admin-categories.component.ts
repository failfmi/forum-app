import { Component, OnInit, Input } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { CategoriesService } from '../../../core/services/categories/categories.service';
import { MatDialog } from '@angular/material';
import { AdminEditCategoryComponent } from '../admin-edit-category/admin-edit-category.component';
import { adminCategoriesAnimations } from './admin-categories.animations';

@Component({
  selector: 'app-admin-categories',
  templateUrl: './admin-categories.component.html',
  styleUrls: ['./admin-categories.component.scss'],
  animations: adminCategoriesAnimations
})
export class AdminCategoriesComponent {
  @Input() categories;
  protected createForm;


  protected pageSize = 6;
  protected currentPage = 1;

  constructor(protected fb: FormBuilder, private categoryService: CategoriesService, public dialog: MatDialog) {
    this.createForm = this.fb.group({
      name: ['', [Validators.required, Validators.minLength(3)]]
    });
  }

  get name() { return this.createForm.get('name'); }

  changePage (page) {
    this.currentPage = page;
  }

  createCategory() {
    const form = this.createForm.value;
    this.createForm.reset();
    this.categoryService.addCategory(form);
  }

  deleteCategory(id) {
    this.categoryService.deleteCategory(id);
  }

  openEditCategoryDialog (category): void {
    const dialogRef = this.dialog.open(AdminEditCategoryComponent, {
      width: '600px',
      height: '300px',
      data: {category}
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
    });
  }
}
