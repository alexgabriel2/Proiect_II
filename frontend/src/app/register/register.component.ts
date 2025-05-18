import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule, Validators} from '@angular/forms';

@Component({
  selector: 'app-register',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './register.component.html',
  standalone: true,
  styleUrl: './register.component.css'
})
export class RegisterComponent {

  private formBuilder=inject(FormBuilder);
  registerForm = this.formBuilder.group({
    firstName: ['',Validators.required],
    lastName: ['',Validators.required],
    username: ['',Validators.required],
    email: ['',[Validators.required,Validators.email]],
    password: ['',[
      Validators.required,
      Validators.minLength(8),
      Validators.pattern(/(?=.*[0-9])(?=.*[a-z])(?=.*[A-Z])/),
    ]],
    confirmPassword: ['',Validators.required]
  },{Validators: this.passwordMatchValidator});
  passwordMatchValidator(form: any) {
    return form.get('password').value === form.get('confirmPassword').value
      ? null : { 'mismatch': true };
  }
  onsubmit(){

  }
}
