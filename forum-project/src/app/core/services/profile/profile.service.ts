import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { LoginHistoryModel } from '../../models/profile/loginHistory.model';
import { Store } from '@ngrx/store';
import { AppState } from '../../store/app.state';
import { History } from '../../store/auth/auth.actions';

@Injectable()
export class ProfileService {
  private userHistory = 'https://localhost:5001/api/account/history';
  constructor(
    private http: HttpClient,
    private store: Store<AppState>) {

  }

  public getUserLoginHistory() {
    this.http.get(this.userHistory)
      .subscribe((history: Array<LoginHistoryModel>) => {
        this.store.dispatch(new History(history));
      });
  }
}
