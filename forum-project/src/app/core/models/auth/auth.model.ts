export class AuthModel {
  constructor(
    public token: string,
    public username: string,
    public email: string,
    public isLoggedIn: boolean,
    public isAdmin: boolean,
    public isBanned: boolean
  ) { }
}
