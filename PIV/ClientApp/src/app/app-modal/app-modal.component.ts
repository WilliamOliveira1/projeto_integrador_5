import { Component, Inject } from '@angular/core';
import { MAT_DIALOG_DATA } from '@angular/material/dialog';
import { ExportDailyWeather, Weather } from '../../angular-model/weather';

@Component({
  selector: 'app-app-modal',
  templateUrl: './app-modal.component.html',
  styleUrls: ['./app-modal.component.css']
})
export class AppModalComponent {
  generalParam: string | undefined;
  disregardedData: string | undefined;
  temperatureMin: string | undefined;
  temperatureMax: string | undefined;
  temperatureAvg: string | undefined;
  temperatureMedian: string | undefined;
  temperatureMode: string | undefined;
  precipitationMax: string | undefined;
  precipitationMin: string | undefined;
  precipitationAvg: string | undefined;
  modalDailyData: ExportDailyWeather;
  weatherData: Weather[];
  constructor(@Inject(MAT_DIALOG_DATA) public data: { daily: ExportDailyWeather, weatherData: Weather[] }) {
    this.modalDailyData = data.daily;
    this.weatherData = data.weatherData;
    this.setModalData();
  }

  setModalData() {
    const daily = this.modalDailyData;
    const day = new Date(daily.infoDate).getDate();
    const month = new Date(daily.infoDate).getMonth();

    let totalElementsWithSimilarDate = this.weatherData.filter(weather =>
      weather.dailyWeatherData.some(daily =>
        new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month
      )
    );
    this.generalParam = totalElementsWithSimilarDate.length.toString();

    let validDataValue = this.weatherData.filter(weather =>
      weather.dailyWeatherData.some(daily =>
        new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month
      ) && weather.dailyWeatherData.some(daily => daily.precipitation > 0 && daily.temperatureC > 0)
    );
    const disregardedValue = totalElementsWithSimilarDate.length - validDataValue.length;
    this.disregardedData = disregardedValue.toString();

    const maxTemperature = Math.max(...validDataValue.flatMap(weather =>
      weather.dailyWeatherData
        .filter(daily => new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month)
        .map(daily => daily.temperatureC)
    ));
    this.temperatureMax = maxTemperature.toString();

    const minTemperature = Math.min(...validDataValue.flatMap(weather =>
      weather.dailyWeatherData
        .filter(daily => new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month && daily.temperatureC > 0)
        .map(daily => daily.temperatureC)
    ));
    this.temperatureMin = minTemperature.toString();

    const temperatures = validDataValue.flatMap(weather =>
      weather.dailyWeatherData
        .filter(daily => new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month)
        .map(daily => daily.temperatureC)
    );

    const averageTemperature = temperatures.reduce((sum, temp) => sum + temp, 0) / temperatures.length;
    this.temperatureAvg = averageTemperature.toString();

    temperatures.sort((a, b) => a - b);
    const mid = Math.floor(temperatures.length / 2);
    const medianTemperature = temperatures.length % 2 !== 0 ? temperatures[mid] : (temperatures[mid - 1] + temperatures[mid]) / 2;
    this.temperatureMedian = medianTemperature.toString();

    const frequency: { [key: number]: number } = {};
    let maxFreq = 0;
    let modeTemperature = temperatures[0];
    for (const temp of temperatures) {
      frequency[temp] = (frequency[temp] || 0) + 1;
      if (frequency[temp] > maxFreq) {
        maxFreq = frequency[temp];
        modeTemperature = temp;
      }
    }
    this.temperatureMode = modeTemperature.toString();

    debugger;
    const maxPrecipitation = Math.max(...this.weatherData.flatMap(weather =>
      weather.dailyWeatherData
        .filter(daily => new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month)
        .map(daily => daily.precipitation)
    ));
    this.precipitationMax = maxPrecipitation.toString();

    const minPrecipitation = Math.min(...this.weatherData.flatMap(weather =>
      weather.dailyWeatherData
        .filter(daily => new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month && daily.precipitation > 0)
        .map(daily => daily.precipitation)
    ));
    this.precipitationMin = minPrecipitation.toString();

    const precipitation = this.weatherData.flatMap(weather =>
      weather.dailyWeatherData
        .filter(daily => new Date(daily.infoDate).getDate() === day && new Date(daily.infoDate).getMonth() === month)
        .map(daily => daily.precipitation)
    );

    const averagePrecipitation = precipitation.reduce((sum, temp) => sum + temp, 0) / precipitation.length;
    this.precipitationAvg = averagePrecipitation.toString();
  }
}
