import { PostModel } from '../../models/posts/post.model';

export interface PostState {
  readonly all: PostModel[];
  readonly postsRequestMade: false;
}
