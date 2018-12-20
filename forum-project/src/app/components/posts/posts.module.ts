import { CommonModule } from '@angular/common';
import { FormsModule, ReactiveFormsModule } from '@angular/forms';
import { NgModule } from '@angular/core';
import { postsComponents } from '.';
import { PostsRoutingModule } from './posts.routing';
import { SharedModule } from '../../shared/shared.module';
import { NgxPaginationModule } from 'ngx-pagination';

@NgModule({
  declarations: [
    ...postsComponents,
  ],
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule,
    PostsRoutingModule,
    SharedModule,
    NgxPaginationModule
  ],
  exports: [
    ...postsComponents
  ],
  entryComponents: [
    ...postsComponents
  ]
})
export class PostsModule { }
