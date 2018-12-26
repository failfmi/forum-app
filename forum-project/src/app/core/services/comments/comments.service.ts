import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AppState } from '../../store/app.state';
import { Store, select } from '@ngrx/store';
import { AddComment, EditComment, DeleteComment } from '../../store/posts/post.actions';
import { ResponseModel } from '../../models/response.model';

const createCommentUrl = 'https://localhost:5001/api/comment/create';
const editCommentUrl = 'https://localhost:5001/api/comment/edit/';
const deleteCommentUrl = 'https://localhost:5001/api/comment/delete/';

@Injectable()
export class CommentsService {
  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private router: Router,
    private store: Store<AppState>) {
  }

  createComment(id: string, model) {
    this.http.post(createCommentUrl, model)
      .subscribe((newCat: ResponseModel) => {
        this.store.dispatch(new AddComment(newCat.data));
        this.toastr.success(newCat.message);
        this.router.navigate([`/posts/details/${id}`]);
    });
  }

  editComment(id: string, model) {
    this.http.put(editCommentUrl + id, model)
      .subscribe((editCat: ResponseModel) => {
        this.store.dispatch(new EditComment(editCat.data));
        this.toastr.info(editCat.message);
    });
  }

  deleteComment(id: string, postId: string) {
    this.http.delete(deleteCommentUrl + id)
      .subscribe((res: ResponseModel) => {
        this.store.dispatch(new DeleteComment(id, postId));
        this.toastr.success(res.message);
    });
  }

  getPostById(id: string): boolean {
    this.store
      .pipe(select(st => st.posts.all))
      .subscribe(posts => {
        if (posts.length > 0) {
          const post = posts.find(p => p.id === id);
          if (!post) {
            return false;
          }
          return true;
        }
          return false;
    });
    return false;
  }
}
