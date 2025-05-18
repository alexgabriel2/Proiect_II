import { Component, OnInit } from '@angular/core';
import { CarService } from '../shared/services/cars.service';
import { CarDto } from '../shared/Models/carDTO';
import {CurrencyPipe, NgForOf} from '@angular/common';

@Component({
  selector: 'app-cars',
  imports: [
    CurrencyPipe,
    NgForOf
  ],
  templateUrl: './cars.component.html',
  standalone: true,
  styleUrls: ['./cars.component.css']
})
export class CarsComponent implements OnInit {
  cars: CarDto[] = [];

  constructor(private carService: CarService) {
  }

  ngOnInit(): void {
    this.carService.getAllCars().subscribe({
      next: (data) => {
        this.cars = data;
        this.cars.forEach((car) => {
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
        console.error('Error fetching cars:', err);
      }
    });
  }
}
