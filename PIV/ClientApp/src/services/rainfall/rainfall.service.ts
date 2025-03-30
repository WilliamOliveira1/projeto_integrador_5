import { HttpClient, HttpParams } from '@angular/common/http';
import { Inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { Weather } from '../../angular-model/weather';

@Injectable({
  providedIn: 'root'
})
export class RainfallService {
  baseApi: string = '';
  constructor(private http: HttpClient, @Inject('BASE_URL') baseUrl: string) {
    this.baseApi = `${baseUrl}precipitationAnalisis`;
  }

  getWeatherData(): Observable<Weather[]> {
    return this.http.get<Weather[]>(this.baseApi);
  }

  //postRainfallData(humidity: string, temperature: string, date: string): Observable<any> {
  postRainfallData(): Observable<any> {
    let params = new HttpParams()
      .set('humidity', 90)
      .set('temperature', 30)
      .set('date', '20/02/2025');

    return this.http.post<any>(`${this.baseApi}`, null, { params });
  }
}
