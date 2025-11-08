using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PIV.Data;
using PIV.interfaces;
using PIV.Models;
using System.Text;
using System.Xml;

namespace PIV.Services
{
    public class RainfallDataFromCsv : IRainfallDataFromCsv
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IConfiguration _configuration;
        ISaveDataFromCsvService _saveDataFromCsvService;
        ISaveDataFromSensor _saveDataFromSensor;
        public RainfallDataFromCsv(IServiceProvider serviceProvider, ISaveDataFromCsvService saveDataFromCsvService, ISaveDataFromSensor saveDataFromSensor, IConfiguration configuration)
        {
            _serviceProvider = serviceProvider;
            _saveDataFromCsvService = saveDataFromCsvService;
            _saveDataFromSensor = saveDataFromSensor;
            _configuration = configuration;
        }

        public List<Weather> getRainfallData()
        {
            if (!_saveDataFromCsvService.HasWeatherData())
            {
                StartRainfallData();
            }
            string idToken = FirebaseLogin().Result;
            if (idToken != null)
            {
                ReadData(idToken).Wait();               
            }            
            return _saveDataFromCsvService.GetAllWeatherData();
        }    
        
        public List<Weather> GetNewestWeatherData()
        {
            return _saveDataFromCsvService.GetNewestWeatherData(100);
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

        async Task ReadData(string idToken)
        {
            using (var client = new HttpClient())
            {
                var firebaseDB = _configuration["FireBase:FirebaseDB"];
                string url = $"{firebaseDB}/sensores.json?auth={idToken}";
                HttpResponseMessage response = await client.GetAsync(url);

                if (response.IsSuccessStatusCode)
                {
                    string responseBody = await response.Content.ReadAsStringAsync();
                    _saveDataFromSensor.SeedData(responseBody);
                }
            }
        }

        private async Task<string?> FirebaseLogin()
        {
            using (var client = new HttpClient())
            {
                var apiKey = _configuration["FireBase:ApiKey"];
                var email = _configuration["FireBase:Email"];
                var password = _configuration["FireBase:Password"];
                string url = $"https://identitytoolkit.googleapis.com/v1/accounts:signInWithPassword?key={apiKey}";

                var payload = new
                {
                    email = email,
                    password = password,
                    returnSecureToken = true
                };

                var content = new StringContent(JsonConvert.SerializeObject(payload), Encoding.UTF8, "application/json");
                HttpResponseMessage response = await client.PostAsync(url, content);

                if (!response.IsSuccessStatusCode)
                {                    
                    return null;
                }

                string responseBody = await response.Content.ReadAsStringAsync();
                JObject json = JObject.Parse(responseBody);
                string idToken = json["idToken"].ToString();
                return idToken;
            }
        }
    }    
}
