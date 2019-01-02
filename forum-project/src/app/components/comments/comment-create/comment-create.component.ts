import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router } from '@angular/router';
import { FormBuilder, Validators } from '@angular/forms';
import { AuthService } from '../../../core/services/auth/auth.service';
import { AppState } from '../../../core/store/app.state';
import { Store, select } from '@ngrx/store';
import { BaseComponent } from '../../base.component';
import { Subscription } from 'rxjs';
import { PostModel } from '../../../core/models/posts/post.model';
import { CommentsService } from '../../../core/services/comments/comments.service';

@Component({
  selector: 'app-comment-create',
  templateUrl: './comment-create.component.html',
  styleUrls: ['./comment-create.component.scss']
})
export class CommentCreateComponent extends BaseComponent implements OnInit {
  protected postId: string;
  protected createForm;
  protected post: PostModel;
  private subscription$: Subscription;

  constructor(private route: ActivatedRoute, protected fb: FormBuilder,
    private authService: AuthService,
    private store: Store<AppState>, private router: Router,
    private commentService: CommentsService) {
      super();
      this.postId = this.route.snapshot.paramMap.get('id');
      this.subscription$ = this.store
        .pipe(select(st => st.posts.all))
        .subscribe(posts => {
          if (posts.length > 0) {
            const post = posts.find(p => Number(p.id) === Number(this.postId));
            if (!post) {
              this.router.navigate(['/home']);
            }
            this.post = post;
          } else {
            this.router.navigate(['/home']);
          }
        });
      this.createForm = this.fb.group({
        text: ['', [Validators.required, Validators.minLength(6), Validators.maxLength(200)]]
      });
      this.subscriptions.push(this.subscription$);
  }

  get text() { return this.createForm.get('text'); }

  ngOnInit() {
  }

  createComment() {
    const form = this.createForm.value;
    form.author = this.authService.userName;
    form.postId = this.post.id;

    this.commentService.createComment(this.post.id, form);
  }
}
