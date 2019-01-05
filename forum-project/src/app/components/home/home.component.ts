import { Component, OnInit } from '@angular/core';
import { BaseComponent } from '../base.component';
import { CategoriesService } from '../../core/services/categories/categories.service';
import { AppState } from '../../core/store/app.state';
import { Store, select } from '@ngrx/store';
import { Subscription } from 'rxjs';
import { CategoryEditModel } from '../../core/models/category/categoryEdit.model';
import { PostsService } from '../../core/services/posts/posts.service';
import { PostHomeModel } from '../../core/models/posts/postHome.model';
import { PostModel } from '../../core/models/posts/post.model';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent extends BaseComponent implements OnInit {
  posts: Array<PostHomeModel> = [];
  categories: CategoryEditModel[];
  leftCatSet: CategoryEditModel[];
  rightCatSet: CategoryEditModel[];

  categorySubscription$: Subscription;
  postSubscription$: Subscription;

  constructor(private categoriesService: CategoriesService, private store: Store<AppState>, private postService: PostsService) {
    super();
  }

  ngOnInit() {
    this.categoriesService.getAllCategories();
    this.postService.getAllPosts();
    this.categorySubscription$ = this.store
      .pipe(select(state => state.categories.all))
      .subscribe(categories => {
        if (categories.length > 0) {
          this.categories = categories.slice();
          this.categories.map(cat => {
            if (cat.name.length > 40) {
              cat.name = cat.name.substr(0, 40) + '...';
            }
            return cat;
          });
          if (this.categories.length > 8) {
            this.categories = this.categories.splice(0, 8);
          }
          this.leftCatSet =
              this.categories.splice(0, Math.ceil(this.categories.length / 2)); // gets the bigger half of the array
          if (this.leftCatSet.length > 5) {
              this.leftCatSet.splice(0, this.leftCatSet.length - 5);
          }
          if (this.categories.length > 5) {
            this.categories.splice(0, 4);
          }
          this.rightCatSet = this.categories;
        }
      });

    this.postSubscription$ = this.store
      .pipe(select(state => state.posts.all))
      .subscribe(posts => {
        this.posts = [];
        const postsS = posts
          .sort((a: PostModel, b: PostModel) => new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime())
          .slice(0, 6);
        for (const i of postsS) {
          const obj = new PostHomeModel(i.id, i.title, i.body, new Date(i.creationDate), i.category.name);
          this.posts.push(obj);
        }
      });
    this.subscriptions.push(this.categorySubscription$);
    this.subscriptions.push(this.postSubscription$);
  }
}
