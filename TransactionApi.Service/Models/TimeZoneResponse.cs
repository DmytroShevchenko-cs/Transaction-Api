using Newtonsoft.Json;

namespace TransactionApi.Service.Models;

public class TimeZoneResponse
{
    [JsonProperty("timezone")] public string Timezone { get; set; } = null!;
}