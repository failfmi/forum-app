import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { ToastrService } from 'ngx-toastr';
import { Router } from '@angular/router';
import { ResponseModel } from '../../models/response.model';

const sendContactUsUrl = 'https://localhost:5001/api/contact-us';

@Injectable()
export class ContactService {

  constructor(
    private http: HttpClient,
    private toastr: ToastrService,
    private router: Router) {
  }

  send(model) {
    this.http.post(sendContactUsUrl, model)
      .subscribe((newCat: ResponseModel) => {
        this.toastr.success(newCat.message);
        this.router.navigate(['/']);
    });
  }
}
