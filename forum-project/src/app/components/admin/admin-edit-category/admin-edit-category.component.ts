import { Component, OnInit, Inject } from '@angular/core';
import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material';
import { FormBuilder, Validators } from '@angular/forms';
import { CategoriesService } from '../../../core/services/categories/categories.service';

@Component({
  selector: 'app-admin-edit-category',
  templateUrl: './admin-edit-category.component.html',
  styleUrls: ['./admin-edit-category.component.scss']
})
export class AdminEditCategoryComponent {
  protected editForm;

  constructor(public dialogRef: MatDialogRef<AdminEditCategoryComponent>,
    @Inject(MAT_DIALOG_DATA) public data, protected fb: FormBuilder, private categoryService: CategoriesService) {
      this.editForm = this.fb.group({
        id: [data.category.id],
        name: [data.category.name, [Validators.required, Validators.minLength(3)]]
      });
    }

  onNoClick(): void {
    this.dialogRef.close();
  }

  get name() { return this.editForm.get('name'); }

  editCategory() {
    const form = this.editForm.value;
    this.categoryService.editCategory(form.id, form);
    this.onNoClick();
  }
}
