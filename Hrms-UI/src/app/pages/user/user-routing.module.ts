import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { HeaderComponent } from '../nav/header/header.component';
import { FooterComponent } from '../nav/footer/footer.component';
import { PhotoComponent } from './photo/photo.component';

const routes: Routes = [
    { path: 'profile', component: ProfileComponent},
    { path: '', redirectTo: 'login', pathMatch: 'full' }
];

@NgModule({
    declarations: [
        ProfileComponent,       
        HeaderComponent,
        FooterComponent,
        PhotoComponent
    ],
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [RouterModule]
})
export class UserRoutingModule { }
