import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AppState } from '../../store/app.state';
import { Store, select } from '@ngrx/store';
import { CategoryEditModel } from '../../models/category/categoryEdit.model';
import { GetAllCategories, Add, Edit, Delete } from '../../store/categories/category.actions';
import { CategoryModel } from '../../models/category/category.model';
import { ResponseModel } from '../../models/response.model';

const allCategoriesUrl = 'http://localhost:5000/category/all';
const createCategoryUrl = 'http://localhost:5000/category/create';
const editCategoryUrl = 'http://localhost:5000/category/edit/';
const deleteCategoryUrl = 'http://localhost:5000/category/delete/';
const cacheTime = 600000; // 10minutes

@Injectable()
export class CategoriesService {

  private cachedCategories = false;
  private lastTimeCalled: number;

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private router: Router,
    private store: Store<AppState>) {
  }

  getAllCategories() {
    const currentTime = new Date().getTime();
    if (this.cachedCategories && (currentTime - this.lastTimeCalled < cacheTime)) {
      return;
    }
    this.lastTimeCalled = currentTime;
    this.cachedCategories = true;

    this.http.get<CategoryEditModel[]>(allCategoriesUrl)
      .subscribe(categories => {
        this.store.dispatch(new GetAllCategories(categories));
      });
  }

  addCategory(model: CategoryModel) {
    this.http.post(createCategoryUrl, model)
      .subscribe((newCat: ResponseModel) => {
        this.store.dispatch(new Add(newCat.data));
        this.toastr.success(newCat.message);
    });
  }

  editCategory(id: string, model: CategoryEditModel) {
    this.http.post(editCategoryUrl + id, model)
      .subscribe((editCat: ResponseModel) => {
        this.store.dispatch(new Edit(editCat.data));
        this.toastr.info(editCat.message);
      });
  }

  deleteCategory(id: string) {
    this.http.delete(deleteCategoryUrl + id)
      .subscribe((res: ResponseModel) => {
        this.store.dispatch(new Delete(id));
        this.toastr.success(res.message);
      });
  }
}
