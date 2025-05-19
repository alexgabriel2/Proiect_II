import {Component} from '@angular/core';
import {FormBuilder, FormGroup, Validators, ReactiveFormsModule} from '@angular/forms';
import {CommonModule} from '@angular/common';
import {AuthService} from '../shared/services/auth.service';


@Component({
  selector: 'app-user-profile',
  standalone: true,
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css'],
  imports: [ReactiveFormsModule, CommonModule]
})
export class UserProfileComponent {
  profileForm: FormGroup;
  passwordForm: FormGroup;

  isModalOpen = false;
  isPasswordModalOpen = false;

  backupData: any;

  loadUserInfo() {
    this.authService.getUserInfo().subscribe({
      next: (user) => {
        this.profileForm.patchValue(user);
      },
      error: (err) => {
        console.log('Failed to load user info', err);
      }
    });
  }


  constructor(private fb: FormBuilder, private authService: AuthService) {
    this.profileForm = this.fb.group({
      firstName: ['asdnasun'],
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
    console.log('Updated Profile:', this.profileForm.value);
    this.closeModal();
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
    const {newPassword, confirmPassword} = this.passwordForm.value;

    if (newPassword !== confirmPassword) {
      alert("Passwords do not match!");
      return;
    }

    // Aici ai trimite parola către backend
    console.log('Password updated:', newPassword);
    this.closePasswordModal();
  }
}
