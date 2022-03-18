using System.Text.Json.Serialization;

namespace Infrastructure.Countries;

public class City
{
    [JsonPropertyName("city")] public string CityName { get; set; }
    [JsonPropertyName("lat")] public string Lat { get; set; }
    [JsonPropertyName("lon")] public string Lon { get; set; }
    [JsonPropertyName("country")] public string CountryName { get; set; }
    [JsonPropertyName("iso2")] public string Iso2 { get; set; }
    [JsonPropertyName("admin_name")] public string AdminName { get; set; }
    [JsonPropertyName("capital")] public string Capital { get; set; }
    [JsonPropertyName("population")] public string Population { get; set; }
    [JsonPropertyName("population_proper")] public string PopulationProper { get; set; }
}