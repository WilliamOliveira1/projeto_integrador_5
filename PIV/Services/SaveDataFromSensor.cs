using CsvHelper.Configuration;
using CsvHelper;
using PIV.Controllers;
using PIV.Data;
using PIV.Mapper;
using PIV.Models;
using System.Globalization;
using System.Text.Json.Nodes;
using PIV.interfaces;
using System.Linq;

namespace PIV.Services
{
    public class SaveDataFromSensor : ISaveDataFromSensor
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WeatherForecastController> _logger;

        public SaveDataFromSensor(ApplicationDbContext context, ILogger<WeatherForecastController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void SeedData(string jsonData)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(jsonData))
                    return;

                var sensorDataList = new List<SensorData>();

                var root = JsonNode.Parse(jsonData);
                if (root == null)
                    return;

                var jsonObject = root.AsObject();
                var dailyGroups = new Dictionary<(string SensorId, DateTime Date), List<SensorData>>();

                foreach (var entry in jsonObject)
                {
                    var sensorId = entry.Key;
                    var data = entry.Value.AsObject();

                    if (!data.ContainsKey("timestamp"))
                        continue;

                    float temperature = data["temperatura"]?.GetValue<float>() ?? 0;
                    float humidity = data["umidade"]?.GetValue<float>() ?? 0;
                    float precipitation = 0;

                    long timestamp = data["timestamp"].GetValue<long>();
                    DateTime infoDate = DateTimeOffset.FromUnixTimeMilliseconds(timestamp).LocalDateTime.Date;

                    if (infoDate.Date == DateTime.Today)
                        continue;

                    var key = (sensorId, infoDate.Date);

                    var sensorData = new SensorData
                    {
                        SensorId = sensorId,
                        TemperatureC = temperature,
                        Humidity = humidity,
                        Precipitation = precipitation,
                        InfoDate = infoDate
                    };

                    if (!dailyGroups.ContainsKey(key))
                        dailyGroups[key] = new List<SensorData>();

                    dailyGroups[key].Add(sensorData);
                }

                var consolidatedDays = dailyGroups
                    .GroupBy(group => group.Key.Date)
                    .ToList();

                 foreach (var dateGroup in consolidatedDays)
                 {
                     var date = dateGroup.Key;
                     // todos os itens daquele dia (de todos os sensores)
                     var allItems = dateGroup.SelectMany(g => g.Value).ToList();
                     if (!allItems.Any())
                         continue;
 
                     // considera o SensorId da última entrada do grupo
                     var lastSensorId = allItems.Last().SensorId;
 
                     bool exists = _context.SensorData.Any(sd => sd.SensorId == lastSensorId && sd.InfoDate.Date == date);
                     if (exists)
                         continue;
 
                     var avgTemp = allItems.Average(x => x.TemperatureC);
                     var avgHumidity = allItems.Average(x => x.Humidity);
                     var avgPrecipitation = allItems.Average(x => x.Precipitation);
 
                     sensorDataList.Add(new SensorData
                     {
                         SensorId = lastSensorId,
                         TemperatureC = avgTemp,
                         Humidity = avgHumidity,
                         Precipitation = avgPrecipitation,
                         InfoDate = date
                     });
                 }

                if (sensorDataList.Count > 0)
                {
                    _context.SensorData.AddRange(sensorDataList);
                    _context.SaveChanges();

                    var weatherList = sensorDataList.Select(sd => SensorDataToWeatherMapper.Map(sd)).ToList();
                    _context.Weather.AddRange(weatherList);
                    _context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.ToString());
            }
        }

        public void DeleteAllSensorData()
        {
            var DeleteAllSensorData = _context.SensorData.ToList();
            _context.SensorData.RemoveRange(DeleteAllSensorData);
            _context.SaveChanges();
        }
    }
}