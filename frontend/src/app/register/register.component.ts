import {Component, inject} from '@angular/core';
import {AbstractControl, FormBuilder, ReactiveFormsModule, ValidatorFn, Validators} from '@angular/forms';
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
  private authService=inject(AuthService);

  private formBuilder = inject(FormBuilder);
  private  isSubmitted: boolean =false;

  passwordMatchValidator: ValidatorFn = (control: AbstractControl): null => {
    const password = control.get('password')
    const confirmPassword = control.get('confirmPassword')

    if (password && confirmPassword && password.value != confirmPassword.value)
      confirmPassword?.setErrors({ mismatch: true })
    else
      confirmPassword?.setErrors(null)

    return null;
  }
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
    confirmPassword: ['']
  }, {validators: this.passwordMatchValidator});




  onsubmit() {
    this.isSubmitted = true;
    if (this.registerForm.valid) {
      this.authService.register(this.registerForm.value).subscribe({
        next:(response:any)=>{
          localStorage.setItem('token',response.accessToken);
          localStorage.setItem('refreshToken',response.refreshToken);
        },
        error: (err) => {
          console.error('Registration failed:', err);
        }
      });
      this.registerForm.reset();
      this.isSubmitted = false;
    }
  }
  hasDisplayableError(controlName: string): Boolean {
    const control = this.registerForm.get(controlName);
    return Boolean(control?.invalid) &&
      (this.isSubmitted || Boolean(control?.touched)|| Boolean(control?.dirty))
  }

}
