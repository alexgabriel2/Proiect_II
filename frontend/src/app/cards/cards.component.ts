import { Component, Input } from '@angular/core';
import { CommonModule, CurrencyPipe, NgForOf } from '@angular/common';
import { CarDto } from '../shared/Models/carDTO';
import { RouterLink } from '@angular/router';

@Component({
  selector: 'app-cards',
  standalone: true,
  imports: [CommonModule, CurrencyPipe, NgForOf, RouterLink],
  templateUrl: './cards.component.html',
  styleUrls: ['./cards.component.css']
})
export class CardsComponent {
  @Input() cars: CarDto[] = [];
}
