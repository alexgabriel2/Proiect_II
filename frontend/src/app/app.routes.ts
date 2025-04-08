import { Routes } from '@angular/router';
import { NgModule } from '@angular/core';
import { RouterModule } from '@angular/router';
import { carsComponent } from './cars/cars.component';
import { AboutComponent } from './about/about.component';

export const routes:
  Routes = [
  { path: 'cars', component: carsComponent },
  { path: 'about', component: AboutComponent },
];

