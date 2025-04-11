// main.ts
import { bootstrapApplication } from '@angular/platform-browser';
import { provideRouter } from '@angular/router';
import { AppComponent } from './app/app.component';
import { HomeComponent } from './app/home/home.component';
import {appConfig} from './app/app.config';



bootstrapApplication(AppComponent, appConfig)
  .catch((err) => console.error(err));
