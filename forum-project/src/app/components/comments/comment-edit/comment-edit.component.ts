import { Component, OnInit, Inject } from '@angular/core';
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material';
import { FormBuilder, Validators } from '@angular/forms';
import { CommentsService } from '../../../core/services/comments/comments.service';

@Component({
  selector: 'app-comment-edit',
  templateUrl: './comment-edit.component.html',
  styleUrls: ['./comment-edit.component.scss']
})
export class CommentEditComponent {
  protected editForm;

  constructor(public dialogRef: MatDialogRef<CommentEditComponent>,
    @Inject(MAT_DIALOG_DATA) public data, protected fb: FormBuilder, private commentService: CommentsService) {
      this.editForm = this.fb.group({
        text: [data.comment.text, [Validators.required, Validators.minLength(6), Validators.maxLength(200)]]
      });
    }

  onNoClick(): void {
    this.dialogRef.close();
  }

  get text() { return this.editForm.get('text'); }

  editComment() {
    this.data.comment.text = this.editForm.value.text;
    this.commentService.editComment(this.data.comment.id, this.data.comment);
    this.onNoClick();
  }
}
