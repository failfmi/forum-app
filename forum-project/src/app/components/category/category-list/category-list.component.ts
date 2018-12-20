import { Component, OnInit, Input } from '@angular/core';
import { CategoryEditModel } from '../../../core/models/category/categoryEdit.model';
import { AppState } from '../../../core/store/app.state';
import { Store } from '@ngrx/store';
import { Select } from '../../../core/store/categories/category.actions';
import { Router } from '@angular/router';

@Component({
  selector: 'app-category-list',
  templateUrl: './category-list.component.html',
  styleUrls: ['./category-list.component.scss']
})
export class CategoryListComponent {
  @Input() protected leftCatSet: CategoryEditModel[];
  @Input() protected rightCatSet: CategoryEditModel[];

  constructor(private store: Store<AppState>, private router: Router) { }

  changeCategory(c) {
    this.store.dispatch(new Select(c));
    this.router.navigate(['/posts']);
  }
}
