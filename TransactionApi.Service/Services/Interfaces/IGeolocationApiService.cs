using TransactionApi.Service.Models;

namespace TransactionApi.Service.Services.Interfaces;

public interface IGeolocationApiService
{
    // Task<string> GetTimeZoneByLocation(string latitude, string longitude, CancellationToken cancellationToken = default);
    
    Task<string> GetClientTimeZone(CancellationToken cancellationToken = default);

    Task<DateTime> ConvertTimeByNames(string coordinateFrom, string coordinateTo, DateTime dateTime, CancellationToken cancellationToken = default);
}