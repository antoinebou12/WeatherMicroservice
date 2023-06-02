using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WeatherMicroservice.Services;
using Microsoft.Extensions.Logging;
using Sentry;

namespace WeatherMicroservice.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly IWeatherService _weatherService;
        private readonly IHub _sentryHub;
        private readonly ILogger<WeatherController> _logger;

        public WeatherController(IWeatherService weatherService, IHub sentryHub, ILogger<WeatherController> logger)
        {
            _weatherService = weatherService;
            _sentryHub = sentryHub;
            _logger = logger;
        }

        [HttpGet]
        public async Task<WeatherData> Get(double latitude, double longitude, double altitude, string town)
        {
            try
            {
                SentrySdk.CaptureMessage("GetWeatherData"); //sentry test
                _logger.LogInformation("GetWeatherData");
                WeatherRequest weatherRequest = new WeatherRequest
                {
                    Latitude = latitude,
                    Longitude = longitude,
                    Altitude = altitude,
                    Town = town
                };

                return await _weatherService.GetWeatherData(weatherRequest);
            }
            catch (System.Exception ex)
            {
                _sentryHub.CaptureException(ex); //sentry test
                throw;
            }
        }
    }
}
