import { NgModule } from '@angular/core';
import { adminComponents } from '.';
import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';
import { AdminRoutingModule } from './admin.routing';
import { MDBBootstrapModule } from 'angular-bootstrap-md';

@NgModule({
  declarations: [
    ...adminComponents
  ],
  imports: [
    AdminRoutingModule,
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule,
    NgxPaginationModule,
    MDBBootstrapModule.forRoot()
  ],
  entryComponents: [
    ...adminComponents
  ]
})
export class AdminModule { }
