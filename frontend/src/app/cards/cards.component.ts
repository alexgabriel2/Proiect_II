import { Component, Input } from '@angular/core';
import { CommonModule, CurrencyPipe, NgForOf } from '@angular/common';
import { CarDto } from '../shared/Models/carDTO';
import { RouterLink } from '@angular/router';
import {FormsModule} from '@angular/forms';
import { CarService } from '../shared/services/cars.service';
@Component({
  selector: 'app-cards',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, NgForOf, RouterLink, FormsModule],
  templateUrl: './cards.component.html',
  styleUrls: ['./cards.component.css']
})
export class CardsComponent {
  @Input() cars: CarDto[] = [];

  isModalOpen = false;
  selectedImageFile: File | null = null;
  constructor(private carService: CarService) {}
  loadCars(): void {
    this.carService.getAllCars().subscribe({
      next: (cars) => {
        this.cars = cars;
      },
      error: (err: any) => {
        console.error('Error loading cars:', err);
      }
    });
  }

  newCar:any = {
    make: '',
    model: '',
    year: new Date().getFullYear(),
    mileage: 0,
    price: 0,
    fuelType: '',
    transmission: '',
    status: 'Available',
    description: '',

  };

  openModal() {
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
    this.resetForm();
  }

  resetForm() {
    this.newCar = {
      make: '',
      model: '',
      year: new Date().getFullYear(),
      mileage: 0,
      price: 0,
      fuelType: '',
      transmission: '',
      status: 'Available',
      description: '',

    };
    this.selectedImageFile = null;
  }

  onImageSelected(event: Event): void {
    const fileInput = event.target as HTMLInputElement;
    if (fileInput.files && fileInput.files.length > 0) {
      this.selectedImageFile = fileInput.files[0];
    }
  }

  submitCar() {
    const formData = new FormData();

    // Adaugă toate câmpurile ca string
    formData.append('make', this.newCar.make);
    formData.append('model', this.newCar.model);
    formData.append('year', this.newCar.year.toString());
    formData.append('mileage', this.newCar.mileage.toString());
    formData.append('price', this.newCar.price.toString());
    formData.append('fuelType', this.newCar.fuelType);
    formData.append('transmission', this.newCar.transmission);
    formData.append('status', this.newCar.status);
    formData.append('description', this.newCar.description);


    if (this.selectedImageFile) {
      formData.append('image', this.selectedImageFile);
    }

    this.carService.addCar(formData).subscribe({
      next: (res) => {
        console.log('Success:', res);
        this.closeModal();
        this.loadCars();
      },
      error: (err) => {
        if (err.status === 200 || err.ok) {
          // fallback pentru cazurile în care răspunsul e tratat greșit ca eroare
          this.closeModal();
          this.loadCars();
        } else {
          console.error('Error adding car:', err);
          alert('Error adding car. Please try again.');
        }
      }
    });
  }

  }
