import { Component } from '@angular/core';
import { FormBuilder, FormGroup, ReactiveFormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-user-profile',
  standalone: true,
  templateUrl: './user-profile.component.html',
  styleUrls: ['./user-profile.component.css'],
  imports: [ReactiveFormsModule, CommonModule]
})
export class UserProfileComponent {
  profileForm: FormGroup;
  isModalOpen = false;
  backupData: any;

  constructor(private fb: FormBuilder) {
    this.profileForm = this.fb.group({
      firstName: ['Amanda'],
      lastName: ['Smith'],
      email: ['amanda@example.com']
    });
  }

  openModal() {
    this.backupData = this.profileForm.value;
    this.isModalOpen = true;
  }

  closeModal() {
    this.isModalOpen = false;
  }

  onUpdate() {
    console.log('Updated Profile:', this.profileForm.value);
    this.closeModal();
  }

  cancelEdit() {
    this.profileForm.setValue(this.backupData);
    this.closeModal();
  }
}
