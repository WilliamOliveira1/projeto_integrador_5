export type Weather = {
  year: number;
  month: number;
  averagePrecipitation: number;
  averageTemperatureC: number;
  dailyWeatherData: ExportDailyWeather[]
}

export type ExportDailyWeather = {
  infoDate: Date;
  precipitation: number,
  temperatureC: number
}
