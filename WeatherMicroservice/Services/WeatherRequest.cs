namespace WeatherMicroservice.Services
{
    public class WeatherRequest
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public string? Town { get; set; }
    }
}
