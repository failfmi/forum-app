import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { sharedComponents } from './index';
import { MatToolbarModule, MatButtonModule, MatFormFieldModule, MatDialogModule,
  MatCardModule,
  MatExpansionModule,
  MatListModule,
  MatInputModule,
  MatSelectModule,
  MatProgressBarModule,
  MatSnackBarModule,
  MatStepperModule,
  MatAutocompleteModule,
  MatBadgeModule,
  MatBottomSheetModule,
  MatButtonToggleModule,
  MatCheckboxModule,
  MatChipsModule,
  MatDatepickerModule,
  MatDividerModule,
  MatGridListModule,
  MatIconModule,
  MatMenuModule,
  MatNativeDateModule,
  MatPaginatorModule,
  MatProgressSpinnerModule,
  MatRadioModule,
  MatRippleModule,
  MatSidenavModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSortModule,
  MatTableModule,
  MatTabsModule,
  MatTooltipModule,
  MatTreeModule, } from '@angular/material';
import { CompareValidatorDirective } from './validators/compare-validator.directive';

const MAT_MODULES = [
  MatButtonModule,
  MatExpansionModule,
  MatInputModule,
  MatToolbarModule,
  MatCardModule,
  MatListModule,
  MatProgressBarModule,
  MatSelectModule,
  MatSnackBarModule,
  MatFormFieldModule,
  MatDialogModule,
  MatStepperModule,
  MatAutocompleteModule,
  MatBadgeModule,
  MatBottomSheetModule,
  MatButtonToggleModule,
  MatCheckboxModule,
  MatChipsModule,
  MatDatepickerModule,
  MatDividerModule,
  MatGridListModule,
  MatIconModule,
  MatMenuModule,
  MatNativeDateModule,
  MatPaginatorModule,
  MatProgressSpinnerModule,
  MatRadioModule,
  MatRippleModule,
  MatSidenavModule,
  MatSliderModule,
  MatSlideToggleModule,
  MatSortModule,
  MatTableModule,
  MatTabsModule,
  MatTooltipModule,
  MatTreeModule,
  ];

@NgModule({
  imports: [
    CommonModule,
    RouterModule,
    ...MAT_MODULES
  ],
  declarations: [
    ...sharedComponents,
    CompareValidatorDirective
  ],
  exports: [
    ...sharedComponents,
    ...MAT_MODULES,
    CompareValidatorDirective
  ]
})
export class SharedModule {

}
