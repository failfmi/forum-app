import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { categoryComponents } from './index';
import { ReactiveFormsModule } from '@angular/forms';
import { SharedModule } from '../../shared/shared.module';

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ReactiveFormsModule,
    SharedModule
  ],
  exports: [
    ...categoryComponents
  ],
  declarations: [
    ...categoryComponents
  ],
  entryComponents: [
    ...categoryComponents
  ]
})
export class CategoryModule {

}
