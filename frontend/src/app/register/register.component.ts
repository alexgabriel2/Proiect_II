import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';
import {AuthService} from '../shared/services/auth.service';
import {NgIf} from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule,
    NgIf
  ],
  templateUrl: './register.component.html',
  standalone: true,
  styleUrl: './register.component.css'
})
export class RegisterComponent {
  constructor(private authService: AuthService) {
  }

  private formBuilder = inject(FormBuilder);
  private  isSubmitted: boolean =false;

  registerForm = this.formBuilder.group({
    username: ['', Validators.required],
    firstName: ['', Validators.required],
    lastName: ['', Validators.required],
    email: ['', [Validators.required, Validators.email]],
    password: ['', [
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])/),
    ]],
    confirmPassword: ['', Validators.required]
  }, {Validators: this.passwordMatchValidator});

  passwordMatchValidator(form: any) {
    return form.get('password').value === form.get('confirmPassword').value
      ? null : {'mismatch': true};
  }


  onsubmit() {
    this.isSubmitted = true;
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next: (response) => {
          console.log('Registration successful:', response);
        },
        error: (err) => {
          console.error('Registration failed:', err);
        }
      });
    }
  }
  hasDisplayableError(controlName: string): Boolean {
    const control = this.registerForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }

}
