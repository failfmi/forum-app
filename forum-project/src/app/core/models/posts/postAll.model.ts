import { CategoryEditModel } from '../category/categoryEdit.model';

export class PostAllModel {
  constructor(
    public _id: string,
    public title: string,
    public body: string,
    public authorName: string,
    public category,
    public creationDate: Date) { }
}
