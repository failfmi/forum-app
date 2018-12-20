import { CategoryEditModel } from '../../models/category/categoryEdit.model';

export interface CategoryState {
  readonly all: CategoryEditModel[];
  readonly selected: CategoryEditModel;
}
