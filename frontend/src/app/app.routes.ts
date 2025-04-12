import { Routes } from '@angular/router';
import { CarsComponent } from './cars/cars.component';
import { AboutComponent } from './about/about.component';
import {RegisterComponent} from './register/register.component';
import {LoginComponent} from './login/login.component';

export const routes: Routes = [

  { path: 'cars', component: CarsComponent },
  { path: 'about', component: AboutComponent },
  { path: '',component: RegisterComponent},
  {path:'login',component:LoginComponent},
];
