import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class AuthService {
  constructor(private http:HttpClient) { }
  baseURL = 'https://localhost:7215/api';
  register(user:any){
    return this.http.post(this.baseURL+'/Auth/Register',user);
  }
  login(user:any){
    return this.http.post(this.baseURL+'/Auth/Login',user);
  }
  getToken() {
    return localStorage.getItem("token");
  }
  isLoggedIn() {
    return this.http.get(this.baseURL+'/User/Validate');
  }
  getUserInfo() {
    return this.http.get(this.baseURL+'/User/GetInfo');
  }
  updateUserInfo(data: any) {
    return this.http.put(this.baseURL+'/User/UpdateProfile',data);
  }
}
