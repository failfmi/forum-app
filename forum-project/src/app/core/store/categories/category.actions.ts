import { Action } from '@ngrx/store';
import { CategoryModel } from '../../models/category/category.model';
import { CategoryEditModel } from '../../models/category/categoryEdit.model';

export const ADD_CATEGORY = '[CATEGORY] ADD_CATEGORY';
export const EDIT_CATEGORY = '[CATEGORY] EDIT_CATEGORY';
export const DELETE_CATEGORY = '[CATEGORY] DELETE_CATEGORY';
export const GET_ALL = '[CATEGORY] GET_All';
export const SELECT = '[CATEGORY] SELECT';

export class GetAllCategories implements Action {
  readonly type: string = GET_ALL;

  constructor(public payload: CategoryEditModel[]) { }
}

export class Add implements Action {
  readonly type: string = ADD_CATEGORY;

  constructor(public payload: CategoryModel) { }
}

export class Edit implements Action {
  readonly type: string = EDIT_CATEGORY;

  constructor(public payload: CategoryEditModel) { }
}

export class Delete implements Action {
  readonly type: string = DELETE_CATEGORY;

  constructor(public id: string) {
  }
}

export class Select implements Action {
  readonly type: string = SELECT;

  constructor(public payload: CategoryEditModel) { }
}

export type Types = GetAllCategories | Add | Edit | Delete;
