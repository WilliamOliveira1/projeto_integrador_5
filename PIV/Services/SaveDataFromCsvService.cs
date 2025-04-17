using CsvHelper;
using CsvHelper.Configuration;
using PIV.Controllers;
using PIV.Data;
using PIV.interfaces;
using PIV.Mapper;
using PIV.Models;
using System.Globalization;

namespace PIV.Services
{
    public class SaveDataFromCsvService : ISaveDataFromCsvService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WeatherForecastController> _logger;

        public SaveDataFromCsvService(ApplicationDbContext context, ILogger<WeatherForecastController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public void SeedDataFromCsv(string csvFilePath)
        {
            try
            {
                if (!_context.Weather.Any())
                {
                    using (var reader = new StreamReader(csvFilePath))
                    using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                    {
                        Delimiter = ",",
                        HasHeaderRecord = true,
                        HeaderValidated = null,
                        MissingFieldFound = null
                    }))
                    {
                        // Map the CSV column "infoDate" to the property "InfoDate" in the Weather class
                        csv.Context.RegisterClassMap<WeatherMap>();

                        var records = csv.GetRecords<Weather>().ToList();
                        Parallel.ForEach(records, record =>
                        {
                            record.ID = 0;
                        });
                        
                        _context.Weather.AddRange(records);
                        _context.SaveChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
            }
        }

        public List<Weather> GetAllWeatherData()
        {
            return _context.Weather.ToList();
        }

        public bool HasWeatherData()
        {
            return _context.Weather.Any();
        }

        public void DeleteAllWeatherData()
        {
            var allWeatherData = _context.Weather.ToList();
            _context.Weather.RemoveRange(allWeatherData);
            _context.SaveChanges();
        }

        public bool SaveWeatherData(Weather wheather)
        {
            try
            {
                if (_context.Weather.Count() < 30000)
                {
                    _context.Weather.Add(wheather);
                    return true;
                }               
            }
            catch (Exception ex)
            {
                _logger.LogDebug(ex.Message);
            }
            return false;
        }
    }
}
