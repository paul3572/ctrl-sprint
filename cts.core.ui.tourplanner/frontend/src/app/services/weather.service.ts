import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { firstValueFrom } from 'rxjs';
import { Weather } from '../contracts/Weather';

@Injectable({ providedIn: 'root' })
export class WeatherService {
  constructor(private readonly http: HttpClient) {}

  getWeather(lat: number, lon: number): Promise<Weather> {
    return firstValueFrom(this.http.get<Weather>(`/api/weather?lat=${lat}&lon=${lon}`));
  }
}
