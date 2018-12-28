import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { NotFoundComponent } from './shared/not-found/not-found.component';
import { HomeComponent } from './components/home/home.component';
import { AboutComponent } from './components/about/about.component';
import { PostsModule } from './components/posts/posts.module';
import { CommentsModule } from './components/comments/comments.module';
import { AdminModule } from './components/admin/admin.module';
import { ContactComponent } from './components/contact/contact.component';

const routes: Routes = [
  { path: '', pathMatch: 'full', redirectTo: 'home'},
  { path: 'about', component: AboutComponent },
  { path: 'home', component: HomeComponent },
  { path: 'contact', component: ContactComponent },
  { path: 'comments', loadChildren: () => CommentsModule },
  { path: 'posts', loadChildren: () => PostsModule },
  { path: 'admin', loadChildren: () => AdminModule},
  { path: '**', component: NotFoundComponent }
];

@NgModule({
  imports: [
    RouterModule.forRoot(routes)
  ],
  exports: [ RouterModule ]
})
export class AppRoutingModule { }
