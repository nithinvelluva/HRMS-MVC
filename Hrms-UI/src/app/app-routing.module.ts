import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent }      from './auth/login/login.component';
import { ForgotpasswordComponent }      from './auth/forgotpassword/forgotpassword.component';

const routes: Routes = [
{ path: 'auth/login', component: LoginComponent },
{ path: 'auth/forgotpassword', component: ForgotpasswordComponent },
{ path: 'user', component: LoginComponent }
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
