import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { ChangepasswordComponent } from './auth/changepassword/changepassword.component';
import { ForgotpasswordComponent } from './auth/forgotpassword/forgotpassword.component';
import { ResetpasswordComponent } from './auth/resetpassword/resetpassword.component';
import { ProfileComponent } from './pages/user/profile/profile.component';
import { UsersComponent } from './pages/admin/users/users.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ChangepasswordComponent,
    ForgotpasswordComponent,
    ResetpasswordComponent,
    ProfileComponent,
    UsersComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
