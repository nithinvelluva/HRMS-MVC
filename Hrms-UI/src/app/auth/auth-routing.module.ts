import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { LoginComponent } from './login/login.component';
import { ChangepasswordComponent } from './changepassword/changepassword.component';
import { ForgotpasswordComponent } from './forgotpassword/forgotpassword.component';
import { ResetpasswordComponent } from './resetpassword/resetpassword.component';
import { HeaderComponent } from './nav/header/header.component';
import { FooterComponent } from './nav/footer/footer.component';

const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'forgotpassword', component: ForgotpasswordComponent },
    { path: 'resetpassword', component: ResetpasswordComponent },
    { path: '', redirectTo: 'login', pathMatch: 'full' }
];

@NgModule({
    declarations: [LoginComponent,
        ChangepasswordComponent,
        ForgotpasswordComponent,
        ResetpasswordComponent,
        HeaderComponent,
        FooterComponent],
    imports: [RouterModule.forChild(routes)],
    exports: [RouterModule]
})
export class AuthRoutingModule { }
