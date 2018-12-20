export class UserModel {
  public _id: string;
  public username: string;
  public email: string;
  public dateRegistered: Date;
  public roles: Array<any>;
  public isBanned: boolean;
}
