import { Component, OnInit, OnDestroy } from '@angular/core';
import { PostModel } from '../../../core/models/posts/post.model';
import { PostsService } from '../../../core/services/posts/posts.service';
import { BaseComponent } from '../../base.component';
import { Subscription } from 'rxjs';
import { AppState } from '../../../core/store/app.state';
import { Store, select } from '@ngrx/store';
import { PostAllModel } from '../../../core/models/posts/postAll.model';
import { Router, ActivatedRoute } from '@angular/router';
import { Select } from '../../../core/store/categories/category.actions';
import { CategoryEditModel } from '../../../core/models/category/categoryEdit.model';
import { CategoriesService } from '../../../core/services/categories/categories.service';

@Component({
  selector: 'app-posts-all',
  templateUrl: './posts-all.component.html',
  styleUrls: ['./posts-all.component.scss']
})
export class PostsAllComponent implements OnDestroy {
  protected posts = [];
  private postSubscription$: Subscription;
  protected pageSize = 6;
  protected currentPage = 1;
  protected postsToShow;
  protected categories;
  protected selectedCategory = 'All';
  protected allValue = 'All';

  constructor(protected postsService: PostsService, private store: Store<AppState>,
    private router: Router, private categoryService: CategoriesService) {
    this.postsService.getAllPosts();
    this.categoryService.getAllCategories();

    this.postSubscription$ = this.store
      .pipe(select(state => state))
      .subscribe(state => {
        this.categories = state.categories.all;
        this.posts = state.posts.all
            .sort((a: PostModel, b: PostModel) => new Date(b.creationDate).getTime() - new Date(a.creationDate).getTime());
        this.posts.forEach(a => a.creationDate = new Date(a.creationDate));
        this.postsToShow = this.posts;
        if (state.categories.selected.hasOwnProperty('id')) {
          this.postsToShow = this.posts.filter(p => p.category.id === state.categories.selected.id);
          this.selectedCategory = state.categories.selected.name;
        } else {
          this.postsToShow = this.posts;
          this.selectedCategory = 'All';
        }
      });
  }

  changePage (page) {
    this.currentPage = page;
  }

  ngOnDestroy(): void {
    this.selectAllCategories();
    this.postSubscription$.unsubscribe();
  }

  navigate(id: string) {
    this.router.navigate([`/posts/details/${id}`]);
  }

  selectAllCategories() {
    this.store.dispatch(new Select(new CategoryEditModel));
  }

  changeCategory(c) {
    this.store.dispatch(new Select(c));
  }
}
