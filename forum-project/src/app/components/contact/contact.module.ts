import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { SharedModule } from '../../shared/shared.module';
import { ContactComponent } from './contact.component';

@NgModule({
  declarations: [
    ContactComponent
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    SharedModule
  ],
  exports: [
    ContactComponent
  ],
  entryComponents: [
    ContactComponent
  ]
})
export class ContactModule { }
