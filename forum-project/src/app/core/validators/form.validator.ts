export class FormValidator {
  public emailValidation =
  '^[a-z0-9!#$%&\'*+/=?^_`{|}~-]+(?:\\.[a-z0-9!#$%&\'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\\.)'
    + '+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?$';

  public phoneValidation = '\\+([0-9\\-]+){5}|\\d([0-9\\-]+){5}';
}
