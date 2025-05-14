import { Component } from '@angular/core';
import {NavbarComponent}  from './shared/navbar/navbar.component';
import { RouterOutlet } from '@angular/router';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet,NavbarComponent],
  template: `
    <app-navbar></app-navbar>
    <router-outlet></router-outlet>`
})
export class AppComponent {
    title(title: any) {
        throw new Error('Method not implemented.');
    }
}
