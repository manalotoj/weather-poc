namespace WeatherFunctionApp
{
    public class Coordinates
    {
        public Coordinates(string lat, string longitude)
        {
            Latitude = lat;
            Longitude = longitude;
        }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
    }
}
