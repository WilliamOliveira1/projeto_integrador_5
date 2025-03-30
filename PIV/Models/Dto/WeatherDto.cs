using PIV.Models;

namespace PIV.Models.Dto
{
    public class WeatherDto
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public float AveragePrecipitation { get; set; }
        public float AverageTemperatureC { get; set; }
        public List<Weather> DailyWeatherData { get; set; }
    }
}
