using PIV.Models;

namespace PIV.Mapper
{
    public class SensorDataToWeatherMapper
    {
        public static Weather Map(SensorData sensorData)
        {
            return new Weather
            {
                InfoDate = sensorData.InfoDate,
                Precipitation = sensorData.Precipitation,
                Humidity = sensorData.Humidity,
                TemperatureC = sensorData.TemperatureC
            };
        }
    }
}
