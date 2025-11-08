using Microsoft.AspNetCore.Mvc;
using PIV.ClientApp.src.services.Prevision;
using PIV.interfaces;
using PIV.Mapper;
using PIV.Models;
using PIV.Models.Dto;
using System.Globalization;

namespace PIV.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class PrecipitationAnalisisController : ControllerBase
    {
        private readonly ILogger<PrecipitationAnalisisController> _logger;
        private readonly IRainfallDataFromCsv _rainfallDataFromCsv;
        private readonly IPrevisionService _previsionService;

        public PrecipitationAnalisisController(ILogger<PrecipitationAnalisisController> logger, IRainfallDataFromCsv rainfallDataFromCsv, IPrevisionService previsionService)
        {
            _logger = logger;
            _rainfallDataFromCsv = rainfallDataFromCsv;
            _previsionService = previsionService;
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

        [HttpGet("getPrevision")]
        [Produces("application/json")]
        public IActionResult GetPrevision([FromQuery] string date)
        {
            if (string.IsNullOrWhiteSpace(date))
                return BadRequest(new { error = "date query parameter is required." });

            if (!DateTime.TryParse(date, out var parsedDate))
            {
                if (!DateTime.TryParseExact(date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
                    return BadRequest(new { error = "invalid date format. Expected yyyy-MM-dd." });
            }

            List<Weather> weatherEntities = _rainfallDataFromCsv.GetNewestWeatherData();
            var result = _previsionService
                 .GenerateContent1(weatherEntities, date, HttpContext.RequestAborted)
                 .GetAwaiter()
                 .GetResult();

            return Ok(result);
        }
    }
}