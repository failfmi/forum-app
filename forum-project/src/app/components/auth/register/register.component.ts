import { Component } from '@angular/core';
import { FormBuilder, Validators, FormGroup } from '@angular/forms';
import { RegisterModel } from '../../../core/models/auth/register.model';
import { AuthService } from '../../../core/services/auth/auth.service';
import { MatDialogRef } from '@angular/material';
import { compareValidator } from '../../../shared/validators/compare-validator.directive';
import { BaseComponent } from '../../base.component';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent extends BaseComponent {
  protected registerForm;

  constructor(protected fb: FormBuilder, public dialogRef: MatDialogRef<RegisterComponent>, private authService: AuthService) {
    super();

    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(4), Validators.pattern('^[^@]*$')]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', [Validators.required, compareValidator('password')]]
    });
  }

  get username() { return this.registerForm.get('username'); }
  get email() { return this.registerForm.get('email'); }
  get password() { return this.registerForm.get('password'); }
  get confirmPassword() { return this.registerForm.get('confirmPassword'); }

  onNoClick(): void {
    this.dialogRef.close();
  }

  register(): void {
    const form = this.registerForm.value;
    const registerModel = new RegisterModel(form.email, form.username, form.password);

    this.authService.register(registerModel)
      .subscribe(() => this.onNoClick());
  }

}
