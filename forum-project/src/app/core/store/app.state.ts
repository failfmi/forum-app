import { AuthState } from './auth/auth.state';
import { CategoryState } from './categories/category.state';
import { PostState } from './posts/post.state';
import { UserState } from './users/users.state';

export interface AppState {
  auth: AuthState;
  categories: CategoryState;
  posts: PostState;
  users: UserState;
}
