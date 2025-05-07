import { Routes } from '@angular/router';
import { CarsComponent } from './cars/cars.component';
import { AboutComponent } from './about/about.component';
import {RegisterComponent} from './register/register.component';
import {LoginComponent} from './login/login.component';
import {DashboardComponent} from './dashboard/dashboard.component';
import {authGuard} from './shared/guard/auth.guard';

export const routes: Routes = [

  { path: 'cars', component: CarsComponent },
  { path: 'about', component: AboutComponent },
  { path: '',component: RegisterComponent},
  { path: 'login',component:LoginComponent},
  { path :'dashboard',component:DashboardComponent,canActivate:[authGuard] },
];
