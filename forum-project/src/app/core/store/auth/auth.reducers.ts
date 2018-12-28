import { AuthState } from './auth.state';
import { AuthModel } from '../../models/auth/auth.model';
import * as AuthActions from './auth.actions';
import { LoginHistoryModel } from '../../models/profile/loginHistory.model';

const initialState: AuthState = {
  auth: new AuthModel('', '', '', false, false, false),
  history: []
};

export function authReducer (
  state: AuthState = initialState,
  action: any) {
  switch (action.type) {
    case AuthActions.AUTH:
      return Object.assign({}, state, {auth: action.payload});
    case AuthActions.HISTORY:
      return Object.assign({}, state, {history: action.payload});
    case AuthActions.LOGOUT:
      return initialState;
    default:
      return state;
  }
}
