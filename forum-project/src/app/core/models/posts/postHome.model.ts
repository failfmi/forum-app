export class PostHomeModel {
  public _id: string;
  public title: string;
  public body: string;
  public creationDate: Date;
  public categoryName: string;

  constructor(id: string, title: string, body: string, creationDate: Date, categoryName: string) {
    this._id = id;
    this.title = title;
    this.body = body;
    this.creationDate = creationDate;
    this.categoryName = categoryName;
  }
}
