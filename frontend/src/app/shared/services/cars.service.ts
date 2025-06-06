import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { CarDto } from '../Models/carDTO';

@Injectable({
  providedIn: 'root'
})
export class CarService {
  private readonly baseURL = 'https://localhost:7215/api' ;

  constructor(private http: HttpClient) {}

  getAllCars(): Observable<CarDto[]> {
    return this.http.get<CarDto[]>(`${this.baseURL}/Cars`);
  }
  getCarImage(carId: string): Observable<Blob> {
    return this.http.get(`${this.baseURL}/Cars/${carId}/Image`, { responseType: 'blob' });
  }
  getCarById(id: string): Observable<CarDto> {
    return this.http.get<CarDto>(`${this.baseURL}/cars/${id}`);
  }
  addCar(formData: FormData): Observable<any> {
    return this.http.post(`${this.baseURL}/Cars/AddCar`, formData);
  }


}
