import { Component, OnInit } from '@angular/core';
import { FormBuilder, Validators } from '@angular/forms';
import { BaseComponent } from '../base.component';
import { ContactService } from 'src/app/core/services/contact/contact.service';

@Component({
  selector: 'app-contact',
  templateUrl: './contact.component.html',
  styleUrls: ['./contact.component.scss']
})
export class ContactComponent extends BaseComponent implements OnInit {
  protected contactForm;

  constructor(private contactUsService: ContactService,
    protected fb: FormBuilder) {
      super();

      this.contactForm = this.fb.group({
        email: ['', [Validators.required, Validators.minLength(3), Validators.maxLength(50)]],
        subject: ['', [Validators.required, Validators.minLength(5), Validators.maxLength(100)]],
        description: ['', [Validators.required, Validators.minLength(20), Validators.maxLength(1000)]]
      });
  }

  get email() { return this.contactForm.get('email'); }

  get subject() { return this.contactForm.get('subject'); }

  get description() { return this.contactForm.get('description'); }

  ngOnInit() {
  }

  submit() {
    const form = this.contactForm.value;

    this.contactUsService.send(form);
  }
}
