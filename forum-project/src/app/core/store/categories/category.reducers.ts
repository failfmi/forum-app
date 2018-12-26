import { CategoryState } from './category.state';
import { CategoryModel } from '../../models/category/category.model';
import { CategoryEditModel } from '../../models/category/categoryEdit.model';
import * as CategoriesActions from './category.actions';

const initialState: CategoryState = {
  all: [],
  selected: new CategoryEditModel
};

function getAllCategories(state, allRecipes) {
  return {
    ...state,
    all: allRecipes
  };
}

function addCategory(state: CategoryState, category: CategoryEditModel) {
  return Object.assign({}, state, {
    all: [...state.all, category]
  });
}

function editCategory(state: CategoryState, category: CategoryEditModel) {
  if (state.all.find(c => c.name === category.name)) {
    return state;
  }
  const copyCats = state.all.slice();
  const categoryToEdit = copyCats.find(c => c.id === category.id);
  categoryToEdit.name = category.name;

  return Object.assign({}, state, {
    all: copyCats
  });
}

function deleteCategory(state: CategoryState, id: string) {
  return Object.assign({}, state, {all: state.all.filter(c => c.id !== id)});
}

function selectCategory(state: CategoryState, payload: CategoryEditModel) {
  return Object.assign({}, state, {selected: payload});
}

export function categoryReducer (
  state: CategoryState = initialState,
  action: any) {
  switch (action.type) {
    case CategoriesActions.GET_ALL:
      return getAllCategories(state, action.payload);
    case CategoriesActions.ADD_CATEGORY:
      return addCategory(state, action.payload);
    case CategoriesActions.EDIT_CATEGORY:
      return editCategory(state, action.payload);
    case CategoriesActions.DELETE_CATEGORY:
      return deleteCategory(state, action.id);
    case CategoriesActions.SELECT:
      return selectCategory(state, action.payload);
    default:
      return state;
  }
}
