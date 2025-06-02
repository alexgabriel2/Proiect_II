import { Component, OnInit } from '@angular/core';
import { CarService } from '../shared/services/cars.service';
import { CarDto } from '../shared/Models/carDTO';
import { CardsComponent } from '../cards/cards.component';
// import { CurrencyPipe, NgForOf } from '@angular/common';


@Component({
  selector: 'app-cars',
  standalone: true,
  templateUrl: './cars.component.html',
  styleUrls: ['./cars.component.css'],
  imports: [
    CardsComponent
  ],
  // imports: [CurrencyPipe, NgForOf, CardsComponent]
})
export class CarsComponent implements OnInit {
  cars: CarDto[] = [];

  constructor(private carService: CarService) {}

  ngOnInit(): void {
    this.carService.getAllCars().subscribe({
      next: (data) => {
        console.log('Cars received from API:', data);
        this.cars = data;


        this.cars.forEach((car) => {

          this.carService.getCarImage(car.id).subscribe({
            next: (imageBlob) => {
              console.log('Image blob for car', car.id, imageBlob); // <- Asta e cheia!
              const reader = new FileReader();
              reader.onload = () => {
                car.image = reader.result as string;
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
