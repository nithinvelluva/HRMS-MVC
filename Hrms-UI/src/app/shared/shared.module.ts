import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { LoaderComponent } from './loader/loader.component';
import { ErrorComponent } from './error/error.component';
import { CalendarComponent } from './calendar/calendar.component';
@NgModule({
  declarations: [CalendarComponent],
  imports: [
    CommonModule
  ],
  exports:[
    LoaderComponent,
    ErrorComponent
  ]
})
export class SharedModule { }
