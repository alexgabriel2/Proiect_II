import { Component, OnInit } from '@angular/core';
import { FavoriteService } from '../shared/services/favorite.service';
import { CarDto } from '../shared/Models/carDTO';
import { CarService } from '../shared/services/cars.service';

@Component({
  selector: 'app-favorite',
  templateUrl: './favorite.component.html',
  standalone: true,
  styleUrls: ['./favorite.component.css']
})
export class FavoriteComponent implements OnInit {
  favoriteCars: CarDto[] = [];

  constructor(private carService: CarService, private favoriteService: FavoriteService) {}

  ngOnInit(): void {
    this.favoriteService.getFavorite().subscribe({
      next: (data) => {
        this.favoriteCars = data;
        this.favoriteCars.forEach((car) => {
          this.carService.getCarImage(car.id).subscribe({
            next: (imageBlob) => {
              const reader = new FileReader();
              reader.onload = () => {
                car.image = reader.result as string; // Convert Blob to base64 string
              };
              reader.readAsDataURL(imageBlob);
            },
            error: (err) => {
              console.error(`Error fetching image for car ID ${car.id}:`, err);
            }
          });
        });
      },
      error: (err) => {
        console.error('Error fetching favorite cars:', err);
      }
    });
  }
}
