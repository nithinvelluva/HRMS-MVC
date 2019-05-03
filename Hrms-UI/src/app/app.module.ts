import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './auth/login/login.component';
import { ChangepasswordComponent } from './auth/changepassword/changepassword.component';
import { ForgotpasswordComponent } from './auth/forgotpassword/forgotpassword.component';
import { HeaderComponent } from './nav/header/header.component';
import { FooterComponent } from './nav/footer/footer.component';

@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ChangepasswordComponent,
    ForgotpasswordComponent,
    HeaderComponent,
    FooterComponent    
  ],
  imports: [
    BrowserModule,
    AppRoutingModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
