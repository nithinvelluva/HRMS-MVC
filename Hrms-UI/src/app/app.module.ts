import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';

import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';

import { AngularFontAwesomeModule } from 'angular-font-awesome';

import { ProfileComponent } from './pages/user/profile/profile.component';
import { UsersComponent } from './pages/admin/users/users.component';
import { PhotoComponent } from './pages/user/photo/photo.component';


@NgModule({
  declarations: [
    AppComponent,   
    ProfileComponent,
    UsersComponent,
    PhotoComponent,
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    AngularFontAwesomeModule
  ],
  providers: [],
  bootstrap: [AppComponent]
})
export class AppModule { }
