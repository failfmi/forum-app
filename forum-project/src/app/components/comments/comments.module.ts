import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { commentsComponents } from '.';
import { CommentsRoutingModule } from './comments.routing';
import { SharedModule } from '../../shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';

@NgModule({
  declarations: [
    ...commentsComponents,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    CommentsRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
  exports: [
    ...commentsComponents
  ],
  entryComponents: [
    ...commentsComponents
  ]
})
export class CommentsModule { }
