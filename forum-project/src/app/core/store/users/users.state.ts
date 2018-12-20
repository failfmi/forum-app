import { UserModel } from '../../models/users/users.model';

export interface UserState {
  readonly all: UserModel[];
}
