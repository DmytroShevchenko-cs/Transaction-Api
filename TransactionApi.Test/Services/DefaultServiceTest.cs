using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NUnit.Framework;
using TransactionApi.DAL;
using TransactionApi.Model;
using TransactionApi.Service.Options;

namespace TransactionApi.Test.Services;

public abstract class DefaultServiceTest<TService> where TService : class
{
    protected IServiceProvider ServiceProvider;
    protected IServiceCollection ServiceCollection;

    public virtual TService Service => ServiceProvider.GetRequiredService<TService>();

    public IConfiguration Configuration;
    
    protected virtual void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<TService>();
        services.AddScoped<HttpClient>();
        services.Configure<TimeZoneApiOption>(Configuration.GetSection("TimeZoneApiOption"));
      
    }
    
    private void SetUpConfiguration()
    {
        Configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"TimeZoneApiOption:BaseUrl", "https://api.ipgeolocation.io/timezone"},
                {"TimeZoneApiOption:ApiKey", "1a7945cb423b45d9a2002cf615a35537"},
            }!)
            .Build();
    }
    
    [SetUp]
    public virtual void SetUp()
    {
        ServiceCollection = new ServiceCollection();
        
        SetUpConfiguration();

        SetUpAdditionalDependencies(ServiceCollection);
        
        ServiceCollection.AddDbContext<TransactionDbContext>(options =>
            options.UseInMemoryDatabase("TestTransactionDB"));
        
        var rootServiceProvider = ServiceCollection.BuildServiceProvider(new ServiceProviderOptions()
            { ValidateOnBuild = true, ValidateScopes = true });

        var spScope = rootServiceProvider.CreateScope();
        ServiceProvider = spScope.ServiceProvider;
    }
}

public abstract class DefaultServiceTest<TServiceInterface, TService> : DefaultServiceTest<TService>
    where TService : class, TServiceInterface where TServiceInterface : class
{
    public override TService Service => ServiceProvider.GetRequiredService<TService>();

    protected override void SetUpAdditionalDependencies(IServiceCollection services)
    {
        services.AddScoped<TServiceInterface, TService>();
        base.SetUpAdditionalDependencies(services);
    }
}