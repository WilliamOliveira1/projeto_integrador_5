using CsvHelper.Configuration;
using PIV.Models;

namespace PIV.Mapper
{
    public sealed class WeatherMap : ClassMap<Weather>
    {
        public WeatherMap()
        {
            Map(m => m.ID).Name("ID");
            Map(m => m.InfoDate).Name("infoDate").TypeConverterOption.Format("yyyy-MM-dd");
            Map(m => m.Precipitation).Name("Precipitation");
            Map(m => m.Humidity).Name("Humidity");
            Map(m => m.TemperatureC).Name("TemperatureC");
        }
    }
}
