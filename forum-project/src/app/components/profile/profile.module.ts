import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { SharedModule } from '../../shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { ProfileService } from '../../core/services/profile/profile.service';
import { profileComponents } from './index';
import { profileRoutes} from './profile.routing';
import { RouterModule, Routes } from '@angular/router';

@NgModule({
  imports: [
    CommonModule,
    SharedModule,
    NgxPaginationModule,
    RouterModule.forChild(profileRoutes)
  ],
  providers: [
    ProfileService
  ],
  declarations: [
    profileComponents
  ]
})
export class ProfileModule {
}
