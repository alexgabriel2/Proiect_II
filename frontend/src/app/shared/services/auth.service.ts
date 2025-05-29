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
    return this.http.get(this.baseURL+'/Auth/Validate');
  }
  getUserInfo() {
    const token = this.getToken();
    const headers = { Authorization: `Bearer ${token}` };
    return this.http.get(`${this.baseURL}/User/GetInfo`, { headers });
  }

  updateUserInfo(data: any) {
    const token = this.getToken();
    const headers = { Authorization: `Bearer ${token}` };
    return this.http.put(`${this.baseURL}/User/UpdateInfo`, data, { headers });
  }
  changePassword(data: any) {
    return this.http.put(`${this.baseURL}/User/ChangePassword`, data);
  }

}
