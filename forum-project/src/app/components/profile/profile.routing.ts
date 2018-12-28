import { Routes } from '@angular/router';
import { MyProfileComponent } from './my-profile/my-profile.component';
import { AuthGuard } from 'src/app/core/guards/auth/auth.guard';

export const profileRoutes: Routes = [
  {
    path: '', component: MyProfileComponent, canActivate: [AuthGuard]
  }
];
