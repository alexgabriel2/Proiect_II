import { Component } from '@angular/core';
import {FavoriteComponent} from '../favorite/favorite.component';

@Component({
  selector: 'app-dashboard',
  imports: [FavoriteComponent],
  templateUrl: './dashboard.component.html',
  standalone: true,
  styleUrl: './dashboard.component.css'
})
export class DashboardComponent {

}
