using PIV.Data;
using PIV.interfaces;
using PIV.Models;

namespace PIV.Services
{
    public class RainfallDataFromCsv : IRainfallDataFromCsv
    {
        private readonly IServiceProvider _serviceProvider;
        ISaveDataFromCsvService _saveDataFromCsvService;
        public RainfallDataFromCsv(IServiceProvider serviceProvider, ISaveDataFromCsvService saveDataFromCsvService)
        {
            _serviceProvider = serviceProvider;
            _saveDataFromCsvService = saveDataFromCsvService;
        }

        public List<Weather> getRainfallData()
        {
            if (!_saveDataFromCsvService.HasWeatherData())
            {
                StartRainfallData();
            }
            return _saveDataFromCsvService.GetAllWeatherData();
        }        

        public void StartRainfallData()
        {
            _ = StartRainfallDataGetterAsync(CancellationToken.None);
        }

        public async Task StartRainfallDataGetterAsync(CancellationToken cancellationToken)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                _saveDataFromCsvService.SeedDataFromCsv("projetointegra03.csv");
            }
            await Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;

        public bool saveNewRainfallData(string humidity, string temperature, string date)
        {
            if (float.TryParse(humidity, out float parsedHumidity) &&
         float.TryParse(temperature, out float parsedTemperature) &&
         DateTime.TryParse(date, out DateTime parsedDate))
            {
                var weather = new Weather
                {
                    Humidity = parsedHumidity,
                    TemperatureC = parsedTemperature,
                    InfoDate = parsedDate
                };

                _saveDataFromCsvService.SaveWeatherData(weather);

                return true;
            }
            return false;
        }
    }    
}
