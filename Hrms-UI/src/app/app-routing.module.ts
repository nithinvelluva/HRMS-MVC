import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { ErrorComponent } from './shared/error/error.component';

const routes: Routes = [
  { path: 'auth', loadChildren: "./auth/auth.module#AuthModule" },
  { path: 'user', loadChildren: "./pages/user/user.module#UserModule" },
  { path: '', redirectTo: 'auth', pathMatch: 'full' },
  { path: '**', component: ErrorComponent }  
];

@NgModule({
  declarations: [ErrorComponent],
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
