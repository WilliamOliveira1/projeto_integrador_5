using System.Text.Json.Serialization;

namespace PIV.Models
{
    public class AIWeatherPrediction
    {
        [JsonPropertyName("average_precipitation")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal AveragePrecipitation { get; set; }

        [JsonPropertyName("average_humidity")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal AverageHumidity { get; set; }

        [JsonPropertyName("average_temperature")]
        [JsonNumberHandling(JsonNumberHandling.AllowReadingFromString)]
        public decimal AverageTemperature { get; set; }
    }
}
