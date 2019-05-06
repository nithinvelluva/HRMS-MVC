import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserRoutingModule } from './user-routing.module';
import { UserdetailsComponent } from './userdetails/userdetails.component';

@NgModule({
  declarations: [UserdetailsComponent],
  imports: [
    CommonModule,
    UserRoutingModule
  ]
})
export class UserModule { }
