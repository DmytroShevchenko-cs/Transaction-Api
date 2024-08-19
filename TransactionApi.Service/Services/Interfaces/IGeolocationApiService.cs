using TransactionApi.Service.Models;

namespace TransactionApi.Service.Services.Interfaces;

public interface IGeolocationApiService
{
    Task<string> GetTimeZoneByLocation(string clientLocation, CancellationToken cancellationToken = default);
    
    Task<string> GetClientTimeZone(CancellationToken cancellationToken = default);

    Task<DateTime> ConvertTimeByCoordinatesAsync(string coordinateFrom, string coordinateTo, DateTime dateTime, CancellationToken cancellationToken = default);
}