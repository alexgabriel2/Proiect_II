import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {AuthService} from '../shared/services/auth.service';
import {Router} from '@angular/router';

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
  private router=inject(Router);
  private authService=inject(AuthService);
  private formBuilder=inject(FormBuilder);
  loginForm = this.formBuilder.group({
    username: [''],
    password: [''],
  });
  onsubmit(){
    this.authService.login(this.loginForm.value).subscribe({
      next:(response:any)=>{
        localStorage.setItem('token',response.accessToken);
        localStorage.setItem('refreshToken',response.refreshToken);
        this.router.navigateByUrl('/user-profile');
      },
      error:(err)=>{
        console.error('Login failed:', err);
      }
    });
  }
}
