import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';

@Component({
  selector: 'app-login',
  imports: [
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  standalone: true,
  styleUrl: './login.component.css'
})
export class LoginComponent {
  private formBuilder=inject(FormBuilder);
  loginForm = this.formBuilder.group({

    email: [''],
    password: [''],

  });
  onsubmit(){

  }
}
