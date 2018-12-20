import { PostState } from './post.state';
import { PostModel } from '../../models/posts/post.model';
import * as PostActions from './post.actions';
import { CommentModel } from '../../models/comments/comment.model';

const initialState: PostState = {
  all: [],
  postsRequestMade: false
};

function getAllPosts(state, allRecipes) {
  return {
    ...state,
    all: allRecipes
  };
}

function addPost(state: PostState, post: PostModel) {
  return Object.assign({}, state, {
    all: [...state.all, post]
  });
}

function editPost(state: PostState, post: PostModel) {
  const copyCats = state.all.slice();
  const postToEdit = copyCats.find(p => p._id === post._id);
  postToEdit.title = post.title;
  postToEdit.body = post.body;
  postToEdit.author = post.author;
  postToEdit.category = post.category;
  postToEdit.creationDate = post.creationDate;
  postToEdit.comments = post.comments;

  return Object.assign({}, state, {
    all: copyCats
  });
}

function addComment(state: PostState, comment: CommentModel) {
  const copyCats = state.all.slice();
  const postToEdit = copyCats.find(p => p._id === comment.postId);
  postToEdit.comments.push(comment);

  return Object.assign({}, state, {
    all: copyCats
  });
}

function editComment(state: PostState, comment: CommentModel) {
  const copyCats = state.all.slice();
  const postToEdit = copyCats.find(p => p._id === comment.postId);
  const commentToEdit = postToEdit.comments.find(c => c._id === comment._id);
  commentToEdit.creationDate = comment.creationDate;
  commentToEdit.creator = comment.creator;
  commentToEdit.text = comment.text;

  return Object.assign({}, state, {
    all: copyCats
  });
}

function deleteComment(state: PostState, id: string, postId: string) {
  const copyCats = state.all.slice();
  const postToEdit = copyCats.find(p => p._id === postId);
  postToEdit.comments = postToEdit.comments.filter(c => c._id !== id);
  return Object.assign({}, state, {
    all: copyCats
  });
}

function deletePost(state: PostState, id: string) {
  return Object.assign({}, state, {all: state.all.filter(c => c._id !== id)});
}

export function postReducer (
  state: PostState = initialState,
  action: any) {
  switch (action.type) {
    case PostActions.GET_ALL:
      return getAllPosts(state, action.payload);
    case PostActions.ADD_POST:
      return addPost(state, action.payload);
    case PostActions.EDIT_POST:
      return editPost(state, action.payload);
    case PostActions.DELETE_POST:
      return deletePost(state, action.id);
    case PostActions.ADD_COMMENT:
      return addComment(state, action.payload);
    case PostActions.EDIT_COMMENT:
      return editComment(state, action.payload);
    case PostActions.DELETE_COMMENT:
      return deleteComment(state, action.id, action.postId);
    case PostActions.GET_ALL_FINISHED:
      return Object.assign({}, state, {
        postsRequestMade: true
      });
    case PostActions.GET_ALL_NOT_FINISHED:
      return Object.assign({}, state, {
        postsRequestMade: false
      });
    default:
      return state;
  }
}
