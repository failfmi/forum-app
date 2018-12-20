import { AuthModel } from '../../models/auth/auth.model';

export interface AuthState {
  readonly auth: AuthModel;
}
