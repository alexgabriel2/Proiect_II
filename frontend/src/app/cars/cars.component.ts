import { Component, OnInit } from '@angular/core';
import { CarService } from '../shared/services/cars.service';
import { CarDto } from '../shared/Models/carDTO';

@Component({
  selector: 'app-cars',
  imports: [],
  templateUrl: './cars.component.html',
  standalone: true,
  styleUrls: ['./cars.component.css']
})
export class CarsComponent implements OnInit {
  cars: CarDto[] = [];

  constructor(private carService: CarService) {}

  ngOnInit(): void {
    this.carService.getAllCars().subscribe({
      next: (data) => {
        this.cars = data;
        this.cars.forEach((car) => {
          this.carService.getCarImage(car.id).subscribe({
            next: (imageUrl) => {
              car.image = imageUrl; // Dynamically add the image property
            },
            error: (err) => {
              console.error(`Error fetching image for car ID ${car.id}:`, err);
            }
          });
        });
      },
      error: (err) => {
        console.error('Error fetching cars:', err);
      }
    });
  }
}
