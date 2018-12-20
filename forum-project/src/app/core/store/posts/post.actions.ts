import { Action } from '@ngrx/store';
import { PostModel } from '../../models/posts/post.model';
import { CommentCreateModel } from '../../models/comments/commentCreate.model';
import { CommentModel } from '../../models/comments/comment.model';

export const ADD_POST = '[POST] ADD_POST';
export const EDIT_POST = '[POST] EDIT_POST';
export const DELETE_POST = '[POST] DELETE_POST';
export const GET_ALL = '[POST] GET_ALL';
export const ADD_COMMENT = '[POST] ADD_COMMENT';
export const EDIT_COMMENT = '[POST] EDIT_COMMENT';
export const DELETE_COMMENT = '[POST] DELETE_COMMENT';
export const GET_ALL_FINISHED = '[POST] GET_ALL_FINISHED';
export const GET_ALL_NOT_FINISHED = '[POST] GET_ALL_NOT_FINISHED';


export class GetAllPosts implements Action {
  readonly type: string = GET_ALL;

  constructor(public payload: PostModel[]) { }
}

export class Add implements Action {
  readonly type: string = ADD_POST;

  constructor(public payload: PostModel) { }
}

export class Edit implements Action {
  readonly type: string = EDIT_POST;

  constructor(public payload: PostModel) { }
}

export class Delete implements Action {
  readonly type: string = DELETE_POST;

  constructor(public id: string) {
  }
}

export class AddComment implements Action {
  readonly type: string = ADD_COMMENT;

  constructor(public payload: CommentCreateModel) { }
}

export class EditComment implements Action {
  readonly type: string = EDIT_COMMENT;

  constructor(public payload: CommentModel) { }
}

export class DeleteComment implements Action {
  readonly type: string = DELETE_COMMENT;

  constructor(public id: string, public postId: string) { }
}

export class GetAllFinished implements Action {
  readonly type: string = GET_ALL_FINISHED;
}

export class GetAllNotFinished implements Action {
  readonly type: string = GET_ALL_NOT_FINISHED;
}

export type Types = GetAllPosts | Add | Edit | Delete | AddComment | EditComment | GetAllFinished | GetAllNotFinished;
