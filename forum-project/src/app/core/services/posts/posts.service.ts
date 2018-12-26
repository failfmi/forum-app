import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { AppState } from '../../store/app.state';
import { Store, select } from '@ngrx/store';
import { GetAllPosts, Add, Edit, Delete, GetAllFinished, GetAllNotFinished } from '../../store/posts/post.actions';
import { PostModel } from '../../models/posts/post.model';
import { ResponseModel } from '../../models/response.model';
import { NgxSpinnerService } from 'ngx-spinner';

const allPostsUrl = 'https://localhost:5001/api/post/all';
const createPostUrl = 'https://localhost:5001/api/post/create';
const editPostUrl = 'https://localhost:5001/api/post/edit/';
const deletePostUrl = 'https://localhost:5001/api/post/delete/';
const cacheTime = 120000; // 2minutes

@Injectable()
export class PostsService {

  private cachedPosts = false;
  private lastTimeCalled: number;

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private router: Router,
    private store: Store<AppState>, private spinner: NgxSpinnerService) {
  }

  getAllPosts() {
    const currentTime = new Date().getTime();

    if (this.cachedPosts && (currentTime - this.lastTimeCalled < cacheTime)) {
      return;
    }
    this.store.dispatch(new GetAllNotFinished());
    this.cachedPosts = true;
    this.lastTimeCalled = currentTime;

    this.spinner.show();
    this.http.get<PostModel[]>(allPostsUrl)
      .subscribe(posts => {
        this.store.dispatch(new GetAllFinished());
        this.store.dispatch(new GetAllPosts(posts));
        this.spinner.hide();
      });
  }

  addPost(model) {
    this.http.post(createPostUrl, model)
      .subscribe((newCat: ResponseModel) => {
        this.store.dispatch(new Add(newCat.data));
        this.toastr.success(newCat.message);
        this.router.navigate(['/posts']);
    });
  }

  editPost(id: string, model: PostModel) {
    this.http.put(editPostUrl + id, model)
      .subscribe((editCat: ResponseModel) => {
        this.store.dispatch(new Edit(editCat.data));
        this.toastr.info(editCat.message);
        this.router.navigate([`/posts/details/${id}`]);
      });
  }

  deletePost(id: string) {
    this.http.delete(deletePostUrl + id)
      .subscribe((res: ResponseModel) => {
        this.store.dispatch(new Delete(id));
        this.toastr.success(res.message);
        this.router.navigate(['/posts']);
    });
  }
}
