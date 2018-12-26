import { CategoryEditModel } from '../category/categoryEdit.model';

export class PostAllModel {
  constructor(
    public id: string,
    public title: string,
    public body: string,
    public author: string,
    public category,
    public creationDate: Date) { }
}
