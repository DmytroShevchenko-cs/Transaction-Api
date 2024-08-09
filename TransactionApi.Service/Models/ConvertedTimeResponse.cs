using Newtonsoft.Json;

namespace TransactionApi.Service.Models;

public class ConvertedTimeResponse
{
    [JsonProperty("converted_time")]
    public DateTime ConvertedTime { get; set; }
}