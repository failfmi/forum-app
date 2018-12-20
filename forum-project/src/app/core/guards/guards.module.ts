import { AuthGuard } from './auth/auth.guard';
import { CommonModule } from '@angular/common';
import { NgModule } from '@angular/core';
import { AdminGuard } from './auth/admin.guard';

@NgModule({
  providers: [
    AuthGuard,
    AdminGuard
  ],
  imports: [
    CommonModule
  ]
})
export class GuardsModule {

}
