using Microsoft.AspNetCore.Mvc;
using PIV.interfaces;
using PIV.Mapper;
using PIV.Models;
using PIV.Models.Dto;

namespace PIV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrecipitationAnalisisController : ControllerBase
    {
        private readonly ILogger<PrecipitationAnalisisController> _logger;
        private readonly IRainfallDataFromCsv _rainfallDataFromCsv;

        public PrecipitationAnalisisController(ILogger<PrecipitationAnalisisController> logger, IRainfallDataFromCsv rainfallDataFromCsv)
        {
            _logger = logger;
            _rainfallDataFromCsv = rainfallDataFromCsv;
        }

        [HttpGet]
        public ActionResult<IEnumerable<WeatherDto>> GetRainfallData()
        {
            List<Weather> weatherEntities = _rainfallDataFromCsv.getRainfallData();
            var weatherDtos = WeatherDtoMap.ToDtoList(weatherEntities);
            return Ok(weatherDtos);
        }

        [HttpPost]
        public ActionResult<IEnumerable<WeatherDto>> PostRainfallData(
            [FromQuery(Name = "humidity")] string humidity, 
            [FromQuery(Name = "temperature")] string temperature, 
            [FromQuery(Name = "date")] string date)
        {
            bool isDataSaved = _rainfallDataFromCsv.saveNewRainfallData(humidity, temperature, date);
            if(isDataSaved)
            {
                return Ok();
            } else
            {
                return BadRequest();
            }
            
        }
    }
}