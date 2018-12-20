import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AdminPanelComponent } from './admin-panel/admin-panel.component';
import { AdminGuard } from '../../core/guards/auth/admin.guard';

const adminRoutes: Routes = [
  { path: '', component: AdminPanelComponent, canActivate: [AdminGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(adminRoutes)],
  exports: [RouterModule]
})
export class AdminRoutingModule { }
