using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeatherMicroservice.Services;

namespace WeatherMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;

        public WeatherController(IWeatherService weatherService)
        {
            _weatherService = weatherService;
        }

        [HttpGet]
        public async Task<WeatherData> Get(double latitude, double longitude, double altitude, string town)
        {
            WeatherRequest weatherRequest = new WeatherRequest 
            { 
                Latitude = latitude, 
                Longitude = longitude, 
                Altitude = altitude, 
                Town = town 
            };

            return await _weatherService.GetWeatherData(weatherRequest);
        }
    }
}
