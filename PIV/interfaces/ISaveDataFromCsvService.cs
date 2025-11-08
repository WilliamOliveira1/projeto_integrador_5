using PIV.Models;

namespace PIV.interfaces
{
    public interface ISaveDataFromCsvService
    {
        void SeedDataFromCsv(string csvFilePath);

        List<Weather> GetAllWeatherData();

        List<Weather> GetNewestWeatherData(int takeCount);

        bool HasWeatherData();

        void DeleteAllWeatherData();

        bool SaveWeatherData(Weather wheather);
    }
}