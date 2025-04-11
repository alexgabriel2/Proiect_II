import { Component } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import {RouterLink} from '@angular/router';

@Component({
  selector: 'app-home',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl:'./home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  isRegisterMode = false; // Start with login form
  firstName = '';
  lastName = '';
  email = '';
  password = '';

  toggleMode() {
    this.isRegisterMode = !this.isRegisterMode;
  }

  onSubmit() {
    if (this.isRegisterMode) {
      console.log('Registration data:', {
        firstName: this.firstName,
        lastName: this.lastName,
        email: this.email,
        password: this.password
      });
      // Add your registration logic here
    } else {
      console.log('Login data:', {
        email: this.email,
        password: this.password
      });
      // Add your login logic here
    }
  }
}
