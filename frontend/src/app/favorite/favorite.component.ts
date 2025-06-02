import { Component, OnInit } from '@angular/core';
import { FavoriteService } from '../shared/services/favorite.service';
import { CarDto } from '../shared/Models/carDTO';
import {NgForOf, NgIf} from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-favorite',
  templateUrl: './favorite.component.html',
  standalone: true,
  imports: [
    NgForOf,
    NgIf,
    RouterLink
  ],
  styleUrls: ['./favorite.component.css']
})
export class FavoriteComponent implements OnInit {
  favoriteCars: CarDto[] = [];

  constructor(private favoriteService: FavoriteService) {}

  ngOnInit(): void {
    this.loadFavorites();
  }

  loadFavorites(): void {
    this.favoriteService.getFavorite().subscribe({
      next: (cars) => {
        this.favoriteCars = cars;
      },
      error: (err) => console.error('Error fetching favorite cars:', err)
    });
  }
  removeFromFavorite(carId: string): void {
    this.favoriteService.removeFromFavorite(carId).subscribe({
      next: () => {
        this.favoriteCars = this.favoriteCars.filter(car => `${car.id}` !== `${carId}`);
      },
      error: (err) => {
        console.error('Error removing car from favorites:', err);
        alert('Eroare la ștergere. Vezi consola.');
      }
    });
  }

}
