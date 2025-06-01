import {Component, inject} from '@angular/core';
import {FormBuilder, ReactiveFormsModule} from '@angular/forms';
import {AuthService} from '../shared/services/auth.service';
import {Router} from '@angular/router';

function getUserIdFromToken(accessToken: any) {
  try {
    const payload = JSON.parse(atob(accessToken.split('.')[1])); // Decode the payload
    return payload["http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"] || null; // Extract userId
  } catch (error) {
    console.error('Error decoding token:', error);
    return null;
  }
}

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
        localStorage.setItem('userId',<string>getUserIdFromToken(response.accessToken));
        this.router.navigateByUrl('/user-profile');
      },
      error:(err)=>{
        console.error('Login failed:', err);
      }
    });
  }

}
