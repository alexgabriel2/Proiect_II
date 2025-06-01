import { Component, OnInit } from '@angular/core';
import { FavoriteService } from '../shared/services/favorite.service';
import { CarDto } from '../shared/Models/carDTO';
import {NgForOf, NgIf} from '@angular/common';

@Component({
  selector: 'app-favorite',
  templateUrl: './favorite.component.html',
  standalone: true,
  imports: [
    NgForOf,
    NgIf
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
}
