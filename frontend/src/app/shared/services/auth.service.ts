import { Injectable } from '@angular/core';
import {HttpClient} from '@angular/common/http';
import {Observable} from 'rxjs';

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
  setToken(token: string) {
    localStorage.setItem("token", token);
  }
  isLoggedIn() {
    return this.http.get(this.baseURL+'/User/Validate');
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

  refreshToken(): Observable<{ newToken: string }> {
    const refreshToken = localStorage.getItem('refreshToken');
    const userId = this.getUserIdFromToken(this.getToken() || '');

    if (!refreshToken || !userId) {
      throw new Error('Missing refresh token or user ID');
    }

    const payload = { userId, refreshToken };
    return this.http.post<{ newToken: string }>(`${this.baseURL}/Auth/RefreshToken`, payload);
  }

  private getUserIdFromToken(token: string): string | null {
    try {
      const payload = JSON.parse(atob(token.split('.')[1]));
      return payload.userId || null;
    } catch {
      return null;
    }
  }
}
