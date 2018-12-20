import { Component, OnInit } from '@angular/core';
import { PostsService } from './core/services/posts/posts.service';
import { CategoriesService } from './core/services/categories/categories.service';
import { Store, select } from '@ngrx/store';
import { AppState } from './core/store/app.state';
import { NgxSpinnerService } from 'ngx-spinner';
import { delay } from 'rxjs/operators';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit {
  title = 'app';
  postsRequestFinished = false;
  constructor(private postService: PostsService,
    private categoryService: CategoriesService, private store: Store<AppState>) {

  }

  ngOnInit(): void {
    // this.postService.getAllPosts();
    // this.categoryService.getAllCategories();
    // this.store
    //   .pipe(select(st => st.posts.postsRequestMade), delay(0))
    //   .subscribe(made => {
    //     this.postsRequestFinished = made;
    //   });
  }
}
