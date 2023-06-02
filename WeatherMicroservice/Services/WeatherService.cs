using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Sentry;

namespace WeatherMicroservice.Services
{
    public class WeatherService : IWeatherService
    {
        private readonly IFeatureManager _featureManager;
        private readonly HttpClient _httpClient;
        private IHub _sentryHub;
        private readonly ILogger<WeatherService> _logger;

        public WeatherService(IFeatureManager featureManager, IHttpClientFactory httpClientFactory, IHub sentryHub, ILogger<WeatherService> logger)
        {
            _featureManager = featureManager;
            _httpClient = httpClientFactory.CreateClient();
            _sentryHub = sentryHub;
            _logger = logger;
        }

        public async Task<WeatherData> GetWeatherData(WeatherRequest request)
        {
            SentrySdk.CaptureMessage("GetWeatherData");
            _logger.LogInformation("GetWeatherData");
            var span = _sentryHub.GetSpan()?.StartChild("GetWeatherData");
            try
            {
                if (await _featureManager.IsEnabledAsync("Mock"))
                {
                    return GetMockData();
                }
                else if (await _featureManager.IsEnabledAsync("Real"))
                {
                    return await FetchRealWeatherData(request);
                }
                else
                {
                    return new WeatherData();
                }
            }
            catch (System.Exception e)
            {
                _sentryHub.CaptureException(e);
                _logger.LogError(e, "GetWeatherDataForLocations");
                throw;
            }
            finally
            {
                span?.Finish();
            }
        }

        private WeatherData GetMockData()
        {
            SentrySdk.CaptureMessage("MockData");
            _logger.LogInformation("MockData");
            return new WeatherData
            {
                Latitude = 51.50853,
                Longitude = -0.12574,
                GenerationTimeMs = 1621636574,
                UtcOffsetSeconds = 3600,
                Timezone = "Europe/London",
                TimezoneAbbreviation = "BST",
                Elevation = 89,
                Time = new List<string> { "2023-05-29T12:00:00Z", "2023-05-29T13:00:00Z" },
                Temperature2m = new List<double> { 16.1, 16.3 },
            };
        }

        private async Task<WeatherData> FetchRealWeatherData(WeatherRequest request)
        {
            SentrySdk.CaptureMessage("FetchRealWeatherData");
            _logger.LogInformation("FetchRealWeatherData");
            var span = _sentryHub.GetSpan()?.StartChild("FetchRealWeatherData");
            try
            {
                var url = $"https://api.open-meteo.com/v1/forecast?latitude={request.Latitude}&longitude={request.Longitude}&hourly=temperature_2m";
                var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);
                var response = await _httpClient.SendAsync(httpRequestMessage);

                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var weatherData = JsonConvert.DeserializeObject<WeatherData>(content);
                    if (weatherData != null)
                    {
                        return weatherData;
                    } else {
                        return new WeatherData();
                    }

                }
                else
                {
                    return new WeatherData();
                }
            }
            catch (System.Exception e)
            {
                _sentryHub.CaptureException(e);
                _logger.LogError(e, "GetWeatherDataForLocations");
                throw;
            }
            finally
            {
                span?.Finish();
            }
        }

        public async Task<IEnumerable<WeatherData>> GetWeatherDataForLocations(IEnumerable<WeatherRequest> requests)
        {
            SentrySdk.CaptureMessage("GetWeatherDataForLocations");
            _logger.LogInformation("GetWeatherDataForLocations");
            var span = _sentryHub.GetSpan()?.StartChild("GetWeatherDataForLocations");
            try
            {
                var tasks = new List<Task<WeatherData>>();
                foreach (var request in requests)
                {
                    tasks.Add(GetWeatherData(request));
                }
                return await Task.WhenAll(tasks);
            }
            catch (System.Exception e)
            {
                _sentryHub.CaptureException(e);
                _logger.LogError(e, "GetWeatherDataForLocations");
                throw;
            }
            finally
            {
                span?.Finish();
            }
        }
    }
}
