import { Component, OnInit, Input } from '@angular/core';
import { Store, select } from '@ngrx/store';
import { AppState } from '../../../core/store/app.state';
import { PostModel } from '../../../core/models/posts/post.model';
import { ActivatedRoute, Router } from '@angular/router';
import { BaseComponent } from '../../base.component';
import { Subscription } from 'rxjs';
import { AuthService } from '../../../core/services/auth/auth.service';
import { PostsService } from '../../../core/services/posts/posts.service';
import { CommentsService } from '../../../core/services/comments/comments.service';
import { MatDialog } from '@angular/material';
import { CommentEditComponent } from '../../comments/comment-edit/comment-edit.component';
import { Select } from '../../../core/store/categories/category.actions';

@Component({
  selector: 'app-post-details',
  templateUrl: './post-details.component.html',
  styleUrls: ['./post-details.component.scss']
})
export class PostDetailsComponent extends BaseComponent {
  protected postId: string;
  protected post: PostModel;
  private subscription$: Subscription;

  constructor(private store: Store<AppState>, private route: ActivatedRoute,
    protected authService: AuthService, private postService: PostsService, private commentService: CommentsService,
    public dialog: MatDialog, private router: Router) {
    super();
    this.postId = this.route.snapshot.paramMap.get('id');
    this.subscription$ = this.store
      .pipe(select(st => st.posts.all))
      .subscribe(posts => {
        if (posts.length > 0) {
          this.post = posts.find(p => p._id === this.postId);
          if (this.post) {
            this.post.creationDate = new Date(this.post.creationDate);
            this.post.comments.forEach(c => c.creationDate = new Date(c.creationDate));
          } else {
            this.router.navigate(['/404']);
          }
        }
      });
      this.subscriptions.push(this.subscription$);
   }

  sameAuthor(): boolean {
    return this.authService.userName === this.post.authorName;
  }

  delete(): void {
    this.postService.deletePost(this.post._id);
  }

  deleteComment(id: string) {
    console.log(id);
    console.log(this.post._id);
    this.commentService.deleteComment(id, this.post._id);
  }

  openEditCommentDialog (comment): void {
    const dialogRef = this.dialog.open(CommentEditComponent, {
      width: '600px',
      height: '300px',
      data: {comment}
    });

    dialogRef.afterClosed().subscribe(result => {
      console.log(result);
    });
  }

  changeCategory(c) {
    this.store.dispatch(new Select(c));
    this.router.navigate(['/posts']);
  }
}
