using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.SwaggerUI;
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

        services.AddTransient<ITransactionService, TransactionService>();
        services.AddTransient<IGeolocationApiService, GeolocationApiService>();
        services.AddTransient<IFileConversionService, FileConversionService>();
        
        services.Configure<TimeZoneApiOption>(Configuration.GetSection("TimeZoneApiOption"));
        services.Configure<DbConnection>(Configuration.GetSection("ConnectionStrings"));
        services.AddTransient<HttpClient>();
        
        services.AddControllers();
        services.AddSwaggerGen();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        //check current project configuration
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction API V1");
            });
        }
        else
        {
            app.UseExceptionHandler("/Error");
            app.UseHsts();
        }

        app.UseRouting();
        app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseStaticFiles();
        
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

}