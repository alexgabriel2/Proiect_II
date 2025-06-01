import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { CarDto } from '../shared/Models/carDTO';
import { CarService } from '../shared/services/cars.service';
import { DomSanitizer, SafeUrl } from '@angular/platform-browser';
import { CommonModule } from '@angular/common';
import {UserService} from '../shared/services/user.service';
import {FavoriteService} from '../shared/services/favorite.service';

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
    private sanitizer: DomSanitizer,
    private userService: UserService,
    private favoriteService: FavoriteService
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
  displaySellerId(): void {
    if (this.carDetails?.sellerId) {
      this.userService.getUserContactInfo(this.carDetails.sellerId).subscribe({
        next: (contactInfo: string) => {
          window.alert(`Seller Contact Info: ${contactInfo}`);
        },
        error: (err) => console.error('Error fetching seller contact info:', err)
      });
    } else {
      console.error('Seller ID not available');
    }
  }
  addToFavorites(): void {
    if (this.carDetails?.id) {
      console.log('Car ID:', this.carDetails.id); // Debugging line
      this.favoriteService.addToFavorite(this.carDetails.id).subscribe({
        next: () => {
          window.alert('Car added to favorites successfully!');
        },
        error: (err) => {
          console.error('Error adding car to favorites:', err);
          window.alert('Failed to add car to favorites. Please try again.');
        }
      });
    } else {
      console.error('Car ID is not available.');
      window.alert('Car details are missing. Cannot add to favorites.');
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
