import { Component } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { LoginModel } from '../../../core/models/auth/login.model';
import { AuthService } from '../../../core/services/auth/auth.service';
import { MatDialogRef } from '@angular/material';
import { BaseComponent } from '../../base.component';
import { AuthService as ExternalAuthService } from 'angularx-social-login';
import { FacebookLoginProvider, GoogleLoginProvider, LinkedInLoginProvider } from 'angularx-social-login';
import { FacebookModel } from 'src/app/core/models/auth/facebook.model';

@Component({
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent extends BaseComponent {
  protected loginForm;

  constructor(protected fb: FormBuilder, public dialogRef: MatDialogRef<LoginComponent>, private authService: AuthService,
    private externalAuthService: ExternalAuthService) {
    super();

    this.loginForm = this.fb.group({
      email: ['', [Validators.required, Validators.email]],
      password: ['', [Validators.required, Validators.minLength(8)]]
    });
   }

  get email() { return this.loginForm.get('email'); }

  get password() { return this.loginForm.get('password'); }

  onNoClick(): void {
    this.dialogRef.close();
  }

  login(): void {
    const form = this.loginForm.value;
    this.authService.login(new LoginModel(form.email, form.password))
      .subscribe(() => this.onNoClick());
  }

  externalLogin(): void {
    this.externalAuthService.signIn(FacebookLoginProvider.PROVIDER_ID)
      .then(result => {
        this.authService.loginFacebook(new FacebookModel(result.authToken))
          .subscribe(() => this.onNoClick());
      });
  }
}
