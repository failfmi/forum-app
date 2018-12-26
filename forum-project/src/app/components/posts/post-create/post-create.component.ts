import { Component, OnInit } from '@angular/core';
import { CategoryEditModel } from '../../../core/models/category/categoryEdit.model';
import { CategoriesService } from '../../../core/services/categories/categories.service';
import { AppState } from '../../../core/store/app.state';
import { Store, select } from '@ngrx/store';
import { BaseComponent } from '../../base.component';
import { FormBuilder, Validators } from '@angular/forms';
import { PostsService } from '../../../core/services/posts/posts.service';
import { AuthService } from '../../../core/services/auth/auth.service';
import { Subscription } from 'rxjs';

@Component({
  selector: 'app-posts-create',
  templateUrl: './post-create.component.html',
  styleUrls: ['./post-create.component.scss']
})
export class PostsCreateComponent extends BaseComponent implements OnInit {
  protected categories: CategoryEditModel[];
  protected createForm;
  private subscription$: Subscription;

  constructor(private categoryService: CategoriesService, private store: Store<AppState>,
    protected fb: FormBuilder, private postService: PostsService, private authService: AuthService) {
    super();

    this.createForm = this.fb.group({
      category: ['', [Validators.required]],
      title: ['', [Validators.required, Validators.minLength(6)]],
      body: ['', [Validators.required, Validators.minLength(10), Validators.maxLength(1000)]]
    });
  }

  ngOnInit() {
    this.categoryService.getAllCategories();
    this.subscription$ = this.store
      .pipe(select(store => store.categories.all))
      .subscribe(categories => {
        this.categories = categories;
      });
    this.subscriptions.push(this.subscription$);
  }

  createPost() {
    const form = this.createForm.value;
    form.categoryId = this.categories.find(c => c.name === form.category).id;

    this.postService.addPost(form);
  }

  get category() { return this.createForm.get('category'); }

  get title() { return this.createForm.get('title'); }

  get body() { return this.createForm.get('body'); }
}
