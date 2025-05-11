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
}
