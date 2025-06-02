import {Component} from '@angular/core';
import {FormBuilder, FormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {AuthService} from '../shared/services/auth.service';
import {FavoriteComponent} from '../favorite/favorite.component';
import { FavoriteService } from '../shared/services/favorite.service';
import {CarDto} from '../shared/Models/carDTO';


@Component({
  selector: 'app-user-profile',
  standalone: true,
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css'],
  imports: [ReactiveFormsModule, CommonModule,FavoriteComponent]
})
export class UserProfileComponent {
  profileForm: FormGroup;
  passwordForm: FormGroup;
  favoriteCars: CarDto[] = [];
  isModalOpen = false;
  isPasswordModalOpen = false;

  backupData: any;

  loadUserInfo() {
    this.authService.getUserInfo().subscribe({
      next: (user) => {
        console.log("USER RECEIVED FROM BACKEND:", user);
        this.profileForm.patchValue(user);
      },
      error: (err) => {
        console.log('Failed to load user info', err);
      }
    });
  }


  constructor(private fb: FormBuilder, private authService: AuthService,private favoriteService: FavoriteService) {
    this.profileForm = this.fb.group({
      firstName: [''],
      lastName: [''],
      username: [''],
      email: ['']
    });

    this.passwordForm = this.fb.group({
      oldPassword: [''],
      newPassword: ['', [Validators.required, Validators.minLength(8)]],
      confirmPassword: ['', Validators.required]
    });
    this.loadUserInfo();
  }


  // Modal pentru profil
  openModal() {
    this.backupData = this.profileForm.value;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  cancelEdit() {
    this.profileForm.setValue(this.backupData);
    this.closeModal();
  }

  onUpdate() {
    if (this.profileForm.invalid) return;

    this.authService.updateUserInfo(this.profileForm.value).subscribe({
      next: (response) => {
        console.log('User updated successfully:', response);
        this.closeModal();  // închide modalul
      },
      error: (err) => {
        console.error('Error updating user:', err);
      }
    });
  }


  // Modal pentru parolă
  openPasswordModal() {
    this.passwordForm.reset();
    this.isPasswordModalOpen = true;
  }

  closePasswordModal() {
    this.isPasswordModalOpen = false;
  }

  onPasswordUpdate() {
    const form = this.passwordForm.value;

    if (form.newPassword !== form.confirmPassword) {
      alert("Passwords do not match");
      return;
    }

    this.authService.changePassword(form).subscribe({
      next: () => {
        alert('Password updated successfully');
        this.closePasswordModal();
      },
      error: (err) => {
        console.error('Password update failed', err);
        alert('Password update failed');
      }
    });
  }

  removeFromFavorites(carId: string): void {
    this.favoriteService.removeFromFavorite(carId).subscribe({
      next: () => {
        this.favoriteCars = this.favoriteCars.filter(car => car.id !== carId);
        alert('Car removed from favorites.');
      },
      error: (err) => {
        console.error('Error removing car from favorites:', err);
      }
    });
  }

}
