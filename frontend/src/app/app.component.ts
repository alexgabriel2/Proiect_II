import { Component } from '@angular/core';
import {RouterOutlet} from '@angular/router';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  imports: [
    RouterOutlet
  ],
  styleUrls: ['./app.component.css']
})
export class AppComponent {
  changeBackgroundColor(event: MouseEvent) {
    const x = event.clientX;
    const y = event.clientY;
    const width = window.innerWidth;
    const height = window.innerHeight;

    // Calculăm culoarea în funcție de poziția mouse-ului
    const red = Math.round((x / width) * 255);
    const green = Math.round((y / height) * 255);
    const blue = Math.round(((x + y) / (width + height)) * 255);

    // Setăm culoarea fundalului
    document.body.style.backgroundColor = `rgb(${red}, ${green}, ${blue})`;
  }
}
