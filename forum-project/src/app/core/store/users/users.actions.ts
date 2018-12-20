import { Action } from '@ngrx/store';
import { UserModel } from '../../models/users/users.model';

export const BAN = '[USER] BAN';
export const UNBAN = '[USER] UNBAN';
export const GET_ALL = '[USER] GET_ALL';

export class GetAllUsers implements Action {
  readonly type: string = GET_ALL;

  constructor(public payload: UserModel[]) { }
}

export class Ban implements Action {
  readonly type: string = BAN;

  constructor(public payload) { }
}

export class Unban implements Action {
  readonly type: string = UNBAN;

  constructor(public payload) { }
}

export type Types = GetAllUsers | Ban | Unban;
