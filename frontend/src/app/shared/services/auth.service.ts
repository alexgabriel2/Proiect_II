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
}
