import { Component, OnInit } from '@angular/core';
import { PostsService } from './core/services/posts/posts.service';
import { CategoriesService } from './core/services/categories/categories.service';
import { Store, select } from '@ngrx/store';
import { AppState } from './core/store/app.state';
import { NgxSpinnerService } from 'ngx-spinner';
import { delay } from 'rxjs/operators';
import { HubConnection, HubConnectionBuilder } from '@aspnet/signalr';
import * as signalR from '@aspnet/signalr';
import { ToastrService } from 'ngx-toastr';
import { Add, Delete } from './core/store/posts/post.actions';
import * as CategoryActions from './core/store/categories/category.actions';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  postsRequestFinished = false;
  private hubConnection: HubConnection;
  constructor(private postService: PostsService,
    private categoryService: CategoriesService, private store: Store<AppState>, private toastService: ToastrService) {

  }

  ngOnInit(): void {
    this.postService.getAllPosts();
    this.categoryService.getAllCategories();
    this.store
      .pipe(select(st => st.posts.postsRequestMade), delay(0))
      .subscribe(made => {
        this.postsRequestFinished = made;
      });

    this.hubConnection = new HubConnectionBuilder()
      .withUrl('https://localhost:5001/api/notify')
      .configureLogging(signalR.LogLevel.Information)
      .build();

      this.hubConnection
        .start()
        .catch(err => console.log(err.toString()));

      this.hubConnection
        .on('PostAdd', (data: any) => {
          this.toastService.info(`A new post has been published: "${data.title}"`);
          this.store.dispatch(new Add(data));
        });

      this.hubConnection
        .on('PostDelete', (data: any) => {
          this.toastService.info(`Post with title "${data.title}" has been deleted.`);
          this.store.dispatch(new Delete(data.id));
        });

      this.hubConnection
        .on('CategoryAdd', (data: any) => {
          this.toastService.info(`A new category has been added: "${data.name}"`);
          this.store.dispatch(new CategoryActions.Add(data));
        });

      this.hubConnection
        .on('CategoryDelete', (data:any) => {
          this.toastService.info(`Category "${data.title}" has been deleted. All related posts will be deleted shortly.`);
          this.store.dispatch(new CategoryActions.Delete(data.id));
        });
  }
}
