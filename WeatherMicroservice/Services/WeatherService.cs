using System.Threading.Tasks;
using Microsoft.FeatureManagement;
using System.Net.Http;
using Newtonsoft.Json;

namespace WeatherMicroservice.Services

{
    public class WeatherService : IWeatherService
    {
        private readonly IFeatureManager _featureManager;
        private readonly HttpClient _httpClient;

        public WeatherService(IFeatureManager featureManager, IHttpClientFactory httpClientFactory)
        {
            _featureManager = featureManager;
            _httpClient = httpClientFactory.CreateClient();
        }

        public async Task<WeatherData> GetWeatherData(WeatherRequest request)
        {
            if (await _featureManager.IsEnabledAsync("Mock"))
            {
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
                    // fill the rest of the properties as needed
                };
            } else if (await _featureManager.IsEnabledAsync("Real"))
            {
                return await FetchRealWeatherData(request);
            }
            else
            {
                return new WeatherData { };
            }
        }

        private async Task<WeatherData> FetchRealWeatherData(WeatherRequest request)
        {
            var url = $"https://api.open-meteo.com/v1/forecast?latitude={request.Latitude}&longitude={request.Longitude}&hourly=temperature_2m";

            var httpRequestMessage = new HttpRequestMessage(HttpMethod.Get, url);

            var response = await _httpClient.SendAsync(httpRequestMessage);

            var content = await response.Content.ReadAsStringAsync();
            System.Console.WriteLine("Response status: " + response.StatusCode);
            System.Console.WriteLine("Response headers: " + response.Headers.ToString());
            System.Console.WriteLine("Response body: " + content);

            if (response.IsSuccessStatusCode)
            {
                var weatherData = JsonConvert.DeserializeObject<WeatherData>(content);
                return weatherData;
            }

            return null;
        }


        public async Task<IEnumerable<WeatherData>> GetWeatherDataForLocations(IEnumerable<WeatherRequest> requests)
        {
            var weatherDataList = new List<WeatherData>();

            foreach (var request in requests)
            {
                var weatherData = await GetWeatherData(request);
                weatherDataList.Add(weatherData);
            }

            return weatherDataList;
        }
    }
}
