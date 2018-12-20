import { CommentModel } from '../comments/comment.model';

export class PostModel {
  public _id: string;
  public title: string;
  public body: string;
  public creationDate;
  public author: string;
  public authorName: string;
  public category;
  public comments: CommentModel[];
}
