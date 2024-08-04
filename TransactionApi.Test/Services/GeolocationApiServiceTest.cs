using System.Globalization;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TransactionApi.Service.Models;
using TransactionApi.Service.Services;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Test.Services;

public class GeolocationApiServiceTest : DefaultServiceTest<IGeolocationApiService, GeolocationApiService>
{
    
    [Test]
    [TestCase("50.12999, 30.65707")]
    public async Task GetClientTimeZone_GetTimeZoneName_Success(string coordinates)
    {
        var resultTimeZone = await Service.GetClientTimeZone();
        Assert.That(resultTimeZone, Is.EqualTo(coordinates));
    }
    
    [Test]
    [TestCase("50.12999, 30.65707", "53.4736827, -77.3977062", "2020-12-08 12:30:00", "2020-12-08 05:30:00")]
    [TestCase("6.602635264, -98.2909591552", "51.110318592, -77.2466440192", "2020-12-08 12:30:00", "2020-12-08 14:30:00")]
    [TestCase("-1.4714172416, -142.375595008", "25.8747932672, -58.2515326976", "2020-12-08 12:30:00", "2020-12-08 17:30:00")]
    [TestCase("10.8277878784, -49.542523904", "-27.20507648, 175.0686826496", "2020-12-08 12:30:00", "2020-12-09 03:30:00")]
    public async Task ConvertTime_GetConvertedTime_Success(string tzFrom, string tzTo, DateTime time, DateTime expectedDateTime)
    {
        var resultDateTime = await Service.ConvertTimeByNames(tzFrom, tzTo, time);
        Assert.That(resultDateTime, Is.EqualTo(expectedDateTime));
    }
    
}