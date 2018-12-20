import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { PostDetailsComponent } from './post-details/post-details.component';
import { PostsAllComponent } from './posts-all/posts-all.component';
import { PostsCreateComponent } from './post-create/post-create.component';
import { AuthGuard } from '../../core/guards/auth/auth.guard';
import { PostEditComponent } from './post-edit/post-edit.component';

const postRoutes: Routes = [
  { path: '', component: PostsAllComponent },
  { path: 'create', component: PostsCreateComponent, canActivate: [AuthGuard] },
  { path: 'details/:id', component: PostDetailsComponent },
  { path: 'edit/:id', component: PostEditComponent, canActivate: [AuthGuard] }
];

@NgModule({
  imports: [RouterModule.forChild(postRoutes)],
  exports: [RouterModule]
})
export class PostsRoutingModule { }
