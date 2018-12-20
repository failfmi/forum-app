import {
  HttpResponse,
  HttpRequest,
  HttpHandler,
  HttpEvent,
  HttpInterceptor
} from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { tap } from 'rxjs/operators';
import { ToastrService } from 'ngx-toastr';
import { Store, select } from '@ngrx/store';
import { AppState } from '../store/app.state';
import * as jwt_decode from 'jwt-decode';
import { AuthModel } from '../models/auth/auth.model';
import { Auth } from '../store/auth/auth.actions';
import { CookieService } from 'ngx-cookie-service';

@Injectable()
export class JWTInterceptor implements HttpInterceptor {
  private token: string;
  constructor(private toastr: ToastrService, private store: Store<AppState>, private cookieService: CookieService) {
    this.store.pipe(select(s => s.auth.auth.token))
      .subscribe(token => this.token = token);
  }
  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    // posts all ?
    if (req.url.endsWith('api/account/login') || req.url.endsWith('/api/account/register') || req.url.endsWith('/api/external/facebook')) {
      req = req.clone({
        setHeaders: {
          'Content-type': 'application/json'
        }
      });
    } else {
      req = req.clone({
        setHeaders: {
          'Authorization': `Bearer ${this.token}`,
          'Content-Type': 'application/json'
        }
      });
    }

    return next.handle(req)
      .pipe(tap((res: HttpEvent<any>) => {
        if (res instanceof HttpResponse && req.url.endsWith('/api/account/login')) {
          this.saveToken(res.body);
        }

        if (res instanceof HttpResponse && req.url.endsWith('/api/external/facebook')) {
          this.saveToken(res.body);
        }

        if (res instanceof HttpResponse && req.url.endsWith('/api/account/register')) {
          this.toastr.success(res.body.message);
        }
      }));
  }

  private saveToken(res) {
    try {
      const decodedToken = jwt_decode(res.token);
      const data = new AuthModel(
                                  res.token,
                                  decodedToken.unique_name,
                                  decodedToken.email,
                                  true,
                                  decodedToken.isAdmin,
                                  decodedToken.isBanned
      );
      this.store.dispatch(new Auth(data));
      this.cookieService.set('token', res.token);
      this.toastr.success(res.message);
    } catch (err) {
      this.toastr.error('Warning! ' + err);
    }
  }

}
