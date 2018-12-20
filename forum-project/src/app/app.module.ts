import { AppRoutingModule } from './app-routing.module';
import { AuthModule } from './components/auth/auth.module';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { BrowserModule } from '@angular/platform-browser';
import { GuardsModule } from './core/guards/guards.module';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { NgModule } from '@angular/core';
import { PostsModule } from './components/posts/posts.module';
import { ServicesModule } from './core/services/services.module';
import { SharedModule } from './shared/shared.module';
import { StoreModule, ActionReducer } from '@ngrx/store';
import { ToastrModule } from 'ngx-toastr';

import { AppComponent } from './app.component';

import { appReducers} from './core/store/app.reducers';
import { ErrorInterceptor, JWTInterceptor } from './core/interceptors';

import { CdkTableModule } from '@angular/cdk/table';
import { CdkTreeModule } from '@angular/cdk/tree';
import { HomeComponent } from './components/home/home.component';

import { MDBBootstrapModule } from 'angular-bootstrap-md';
import { AboutComponent } from './components/about/about.component';
import { CookieService } from 'ngx-cookie-service';

import { environment } from '../environments/environment';
import { storeLogger } from 'ngrx-store-logger';
import { AppState } from './core/store/app.state';
import { CategoryModule } from './components/category/category.module';
import { CommentsModule } from './components/comments/comments.module';
import { NgxSpinnerModule } from 'ngx-spinner';

export function logger(reducer: ActionReducer<AppState>): any {
  return storeLogger()(reducer);
}

export const metaReducers = environment.production ? [] : [logger];

@NgModule({
  declarations: [
    AppComponent,
    HomeComponent,
    AboutComponent,
  ],
  imports: [
    AppRoutingModule,
    AuthModule,
    BrowserAnimationsModule,
    BrowserModule,
    CategoryModule,
    CommentsModule,
    GuardsModule,
    HttpClientModule,
    PostsModule,
    ServicesModule,
    SharedModule,
    StoreModule.forRoot(appReducers, {metaReducers}),
    ToastrModule.forRoot(),
    MDBBootstrapModule.forRoot(),
    NgxSpinnerModule
  ],
  providers: [
    {
      provide: HTTP_INTERCEPTORS,
      useClass: ErrorInterceptor,
      multi: true
    },
    {
      provide: HTTP_INTERCEPTORS,
      useClass: JWTInterceptor,
      multi: true
    },
    CookieService
  ],
  exports: [
    CdkTableModule,
    CdkTreeModule,
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
