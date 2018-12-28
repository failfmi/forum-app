import { Action } from '@ngrx/store';
import { AuthModel } from '../../models/auth/auth.model';
import { LoginHistoryModel } from '../../models/profile/loginHistory.model';

export const AUTH = '[AUTH] Auth';
export const LOGOUT = '[AUTH] Logout';
export const HISTORY = '[AUTH] History';

export class Auth implements Action {
  readonly type: string = AUTH;

  constructor(public payload: AuthModel) { }
}

export class History implements Action {
  readonly type: string = HISTORY;

  constructor(public payload: Array<LoginHistoryModel>) { }
}

export class Logout implements Action {
  readonly type: string = LOGOUT;

  constructor() { }
}

export type Types = Auth | History | Logout;
