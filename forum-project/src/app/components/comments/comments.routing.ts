import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { AuthGuard } from '../../core/guards/auth/auth.guard';
import { CommentCreateComponent } from './comment-create/comment-create.component';
import { CommentEditComponent } from './comment-edit/comment-edit.component';

const commentRoutes: Routes = [
  { path: 'create/:id', component: CommentCreateComponent, canActivate: [AuthGuard] },
];

@NgModule({
  imports: [RouterModule.forChild(commentRoutes)],
  exports: [RouterModule]
})
export class CommentsRoutingModule { }
