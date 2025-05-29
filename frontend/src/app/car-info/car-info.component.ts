import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CarDto } from '../shared/Models/carDTO';
import { CarService } from '../shared/services/cars.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-car-info',
  standalone: true,
  templateUrl: './car-info.component.html',
  styleUrls: ['./car-info.component.css'],
  imports: [CommonModule]
})
export class CarInfoComponent implements OnInit {
  carId: string = '';
  carDetails?: CarDto;
  carImageUrl?: SafeUrl;

  constructor(
    private route: ActivatedRoute,
    private carService: CarService,
    private sanitizer: DomSanitizer
  ) {}

  ngOnInit(): void {
    this.carId = this.route.snapshot.paramMap.get('id') ?? '';
    console.log('Car ID din URL:', this.carId); // VERIFICARE!

    if (this.carId) {
      this.carService.getCarById(this.carId).subscribe({
        next: (car) => {
          console.log('Date mașină primite:', car); // VERIFICARE!
          this.carDetails = car;
          this.loadCarImage(this.carId);
        },
        error: (err) => console.error('Eroare la preluarea detaliilor mașinii:', err)
      });
    }
  }

  loadCarImage(id: string): void {
    this.carService.getCarImage(id).subscribe({
      next: (blob) => {
        const objectURL = URL.createObjectURL(blob);
        this.carImageUrl = this.sanitizer.bypassSecurityTrustUrl(objectURL);
      },
      error: (err) => console.error('Error loading car image:', err)
    });
  }
}
