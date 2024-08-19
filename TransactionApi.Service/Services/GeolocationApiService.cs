using System.Globalization;
using System.Text;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using TransactionApi.Service.Models;
using TransactionApi.Service.Options;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Service.Services;

public class GeolocationApiService(IOptions<TimeZoneApiOption> apiOption, HttpClient httpClient)
    : IGeolocationApiService
{
    private readonly TimeZoneApiOption _apiOption = apiOption.Value;
    private readonly HttpClient _httpClient = httpClient;

    /// <summary>
    /// Returns timezone using location coordinates
    /// </summary>
    public async Task<string> GetTimeZoneByLocation(string clientLocation, CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder(_apiOption.BaseUrl);
        
        var coords = clientLocation.Split(",", StringSplitOptions.TrimEntries);
        
        if (coords.Length != 2)
            throw new Exception("Error with coordinates format");
        url.Append($"?apiKey={_apiOption.ApiKey}&lat={coords[0]}&long={coords[1]}");
        
        var response = await _httpClient.GetStringAsync(url.ToString(), cancellationToken);
        
        var data = JsonConvert.DeserializeObject<TimeZoneResponse>(response);
        if (data is null)
            throw new Exception("Response error");
        
        return data.Timezone;
    }

    /// <summary>
    /// Returns user`s timezone
    /// </summary>
    public async Task<string> GetClientTimeZone(CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder(_apiOption.BaseUrl);
        url.Append($"?apiKey={_apiOption.ApiKey}");
    
        var response = await _httpClient.GetStringAsync(url.ToString(), cancellationToken);
        
        var data = JsonConvert.DeserializeObject<CoordinatesResponse>(response);
        if (data is null)
            throw new Exception("Response error");
        
        return data.Timezone;
    }
    
    /// <summary>
    /// Returns converted date and time
    /// </summary>
    public async Task<DateTime> ConvertTimeByCoordinatesAsync(string coordinatesFrom, string coordinatesTo, DateTime dateTime, CancellationToken cancellationToken = default)
    {
        var url = new StringBuilder(_apiOption.BaseUrl);

        var coordsFrom = coordinatesFrom.Split(",", StringSplitOptions.TrimEntries);
        var coordsTo = coordinatesTo.Split(",", StringSplitOptions.TrimEntries);

        if (coordsTo.Length != 2 || coordsFrom.Length != 2)
            throw new Exception("Error with coordinates format");
        
        url.Append($"/convert?apiKey={_apiOption.ApiKey}&lat_from={coordsFrom[0]}&long_from={coordsFrom[1]}&lat_to={coordsTo[0]}&long_to={coordsTo[1]}&time={dateTime.ToString("yyyy-MM-dd HH:mm:ss")}");

        var response = await _httpClient.GetStringAsync(url.ToString(), cancellationToken);
        
        var data = JsonConvert.DeserializeObject<ConvertedTimeResponse>(response);
        if (data is null)
            throw new Exception("Response error");
        
        return data.ConvertedTime;
    }
}