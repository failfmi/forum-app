import { AuthModel } from '../../models/auth/auth.model';
import { LoginHistoryModel } from '../../models/profile/loginHistory.model';

export interface AuthState {
  readonly auth: AuthModel;
  readonly history: Array<LoginHistoryModel>;
}
