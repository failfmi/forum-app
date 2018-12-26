import { UserState } from './users.state';
import * as UsersActions from './users.actions';
import { LOGOUT } from '../auth/auth.actions';

const initialState: UserState = {
  all: []
};

function getAllUsers(state, allUsers) {
  return {
    ...state,
    all: allUsers
  };
}

function banUser(state: UserState, payload) {
  const copyUsers = state.all.slice();
  const user = copyUsers.find(u => u.id === payload.id);
  user.isBanned = payload.isBanned;
  return Object.assign({}, state, {
    all: copyUsers
  });
}

function unbanUser(state: UserState, payload) {
  const copyUsers = state.all.slice();
  const user = copyUsers.find(u => u.id === payload.id);
  user.isBanned = payload.isBanned;
  return Object.assign({}, state, {
    all: copyUsers
  });
}

export function userReducer (
  state: UserState = initialState,
  action: any) {
  switch (action.type) {
    case UsersActions.GET_ALL:
      return getAllUsers(state, action.payload);
    case UsersActions.BAN:
      return banUser(state, action.payload);
    case UsersActions.UNBAN:
      return unbanUser(state, action.payload);
    case LOGOUT:
      return initialState;
    default:
      return state;
  }
}
