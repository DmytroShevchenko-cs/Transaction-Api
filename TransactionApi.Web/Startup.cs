using Microsoft.EntityFrameworkCore;
using TransactionApi.Model;
using TransactionApi.Service.Options;
using TransactionApi.Service.Services;
using TransactionApi.Service.Services.Interfaces;

namespace TransactionApi.Web;

public class Startup
{
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        var connectionString = Environment.GetEnvironmentVariable("SQLSERVER_CONNECTION_STRING") ?? Configuration.GetConnectionString("ConnectionString");

        services.AddDbContext<TransactionDbContext>(options =>
            options.UseSqlServer(connectionString));

        services.AddTransient<IGeolocationApiService, GeolocationApiService>();
        
        services.Configure<TimeZoneApiOption>(Configuration.GetSection("TimeZoneApiOption"));
        services.AddTransient<HttpClient>();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //check current project configuration
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }
        
        app.UseRouting();
        app.UseStaticFiles();
    }

}