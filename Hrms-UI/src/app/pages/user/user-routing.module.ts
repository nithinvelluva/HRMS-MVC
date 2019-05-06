import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ProfileComponent } from './profile/profile.component';
import { HeaderComponent } from '../nav/header/header.component';
import { FooterComponent } from '../nav/footer/footer.component';
import { PhotoComponent } from './photo/photo.component';
import { SidebarComponent } from '../nav/sidebar/sidebar.component';
import { UserdetailsComponent } from './userdetails/userdetails.component';

const routes: Routes = [
    { path: 'profile', component: ProfileComponent},
    { path: '', redirectTo: 'login', pathMatch: 'full' }
];

@NgModule({
    declarations: [
        ProfileComponent,       
        HeaderComponent,
        FooterComponent,
        PhotoComponent,
        SidebarComponent,
        UserdetailsComponent
    ],
    imports: [
        RouterModule.forChild(routes)
    ],
    exports: [RouterModule]
})
export class UserRoutingModule { }
