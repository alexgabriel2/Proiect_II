import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CarDto } from '../Models/carDTO';


@Injectable({
  providedIn: 'root'
})
export class FavoriteService {
  private baseURL = 'https://localhost:7215/api/User/Favorite';

  constructor(private http: HttpClient) {}

  getFavorite(): Observable<CarDto[]> {
    return this.http.get<CarDto[]>(`${this.baseURL}/Get`);
  }
  addToFavorite(carId: string): Observable<any> {
    return this.http.post(`${this.baseURL}/Add`, { carId }, { responseType: 'text' });
  }

  removeFromFavorite(carId: string): Observable<any> {
    return this.http.delete(`${this.baseURL}/Delete?carId=${carId}`, {
      responseType: 'text'
    });
  }






}
