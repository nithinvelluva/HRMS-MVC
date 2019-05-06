import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { UserModule } from './user/user.module';
import { SidebarComponent } from './nav/sidebar/sidebar.component';
@NgModule({
  declarations: [
   
  SidebarComponent],
  imports: [
    CommonModule,
    UserModule
  ],
  providers: []
})
export class PagesModule { }
