import { AuthState } from './auth.state';
import { AuthModel } from '../../models/auth/auth.model';
import * as AuthActions from './auth.actions';

const initialState: AuthState = {
  auth: new AuthModel('', '', '', false, false, false)
};

export function authReducer (
  state: AuthState = initialState,
  action: any) {
  switch (action.type) {
    case AuthActions.AUTH:
      return Object.assign({}, state, {auth: action.payload});
    case AuthActions.LOGOUT:
      return initialState;
    default:
      return state;
  }
}
