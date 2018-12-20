import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { authComponents } from './index';
import { ReactiveFormsModule } from '@angular/forms';
import { MatToolbarModule, MatButtonModule, MatFormFieldModule, MatDialogModule,
  MatCardModule,
  MatExpansionModule,
  MatListModule,
  MatInputModule,
  MatSelectModule,
  MatProgressBarModule,
  MatSnackBarModule } from '@angular/material';

import { SocialLoginModule, AuthServiceConfig } from 'angularx-social-login';
import { GoogleLoginProvider, FacebookLoginProvider, LinkedInLoginProvider} from 'angularx-social-login';

const config = new AuthServiceConfig([
  {
    id: FacebookLoginProvider.PROVIDER_ID,
    provider: new FacebookLoginProvider('295295937773610')
  }
]);

export function provideConfig() {
  return config;
}


const MAT_MODULES = [
  MatButtonModule,
  MatExpansionModule,
  MatInputModule,
  MatToolbarModule,
  MatCardModule,
  MatListModule,
  MatProgressBarModule,
  MatSelectModule,
  MatSnackBarModule,
  MatFormFieldModule,
  MatDialogModule
  ];

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    ...MAT_MODULES,
    SocialLoginModule
  ],
  exports: [
    ...authComponents
  ],
  declarations: [
    ...authComponents
  ],
  entryComponents: [
    ...authComponents
  ],
  providers: [
    {
      provide: AuthServiceConfig,
      useFactory: provideConfig
    }
  ]
})
export class AuthModule {

}
