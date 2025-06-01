import { Injectable } from '@angular/core';
import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class UserService {
  private baseUrl = 'https://localhost:7215/api/';

  constructor(private http: HttpClient) {}


  getUserContactInfo(userId: string): Observable<string> {
    return this.http.get(`${this.baseUrl}User/UserContact?userId=${userId}`, { responseType: 'text' });
  }
}
