using Newtonsoft.Json;

namespace TransactionApi.Service.Models;

public class CoordinatesResponse
{
    [JsonProperty("geo")]
    public Geo Geo { get; set; } = null!;
    
    [JsonProperty("timezone")]
    public string Timezone { get; set; } = null!;
}
public class Geo
{
    [JsonProperty("latitude")]
    private string Latitude { get; set; } = null!;
    
    [JsonProperty("longitude")]
    private string Longitude { get; set; } = null!;

    public override string ToString()
    {
        return $"{Latitude}, {Longitude}";
    }
}