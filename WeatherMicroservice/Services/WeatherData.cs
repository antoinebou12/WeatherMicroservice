namespace WeatherMicroservice.Services
{
    public class WeatherData
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? GenerationTimeMs { get; set; }
        public int? UtcOffsetSeconds { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public double? Elevation { get; set; }
        public List<string>? Time { get; set; }
        public List<double>? Temperature2m { get; set; }
        // add more properties as per your JSON structure
    }

        public class WeatherApiResponse
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? GenerationTimeMs { get; set; }
        public int? UtcOffsetSeconds { get; set; }
        public string? Timezone { get; set; }
        public string? TimezoneAbbreviation { get; set; }
        public double? Elevation { get; set; }
        public HourlyUnits? HourlyUnits { get; set; }
        public WeatherData? Hourly { get; set; }
    }

    public class HourlyUnits
    {
        public string? time { get; set; }
        public string? temperature_2m { get; set; }
    }

    public class Hourly
    {
        public List<string>? time { get; set; }
        public List<double>? temperature_2m { get; set; }
    }
}
