namespace WeatherMicroservice.Services
{
    public interface IWeatherService
    {
        Task<WeatherData> GetWeatherData(WeatherRequest request);
    }
}