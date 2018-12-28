import { Component, OnInit } from '@angular/core';
import { Subscription } from 'rxjs';
import { ProfileService } from 'src/app/core/services/profile/profile.service';
import { Store, select } from '@ngrx/store';
import { AppState } from 'src/app/core/store/app.state';
import { BaseComponent } from '../../base.component';

@Component({
  selector: 'app-my-profile',
  templateUrl: './my-profile.component.html',
  styleUrls: ['./my-profile.component.scss']
})
export class MyProfileComponent extends BaseComponent implements OnInit {
  protected history = [];
  private historySubscription$: Subscription;
  protected pageSize = 6;
  protected currentPage = 1;

  constructor(protected profileService: ProfileService, private store: Store<AppState>) {
    super();
    this.profileService.getUserLoginHistory();

    this.historySubscription$ = this.store
      .pipe(select(state => state.auth.history))
      .subscribe(history => {
        this.history = history;
        this.history.forEach(hist => hist = new Date(hist));
      });
    this.subscriptions.push(this.historySubscription$);
   }

  ngOnInit() {
  }

  changePage (page) {
    this.currentPage = page;
  }
}
