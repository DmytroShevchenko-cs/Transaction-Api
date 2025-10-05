using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
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
        var connectionString = Environment.GetEnvironmentVariable("CONNECTION_STRING") ?? Configuration.GetConnectionString("ConnectionString");

        services.AddDbContext<TransactionDbContext>(options =>
            options.UseNpgsql(connectionString));

        services.AddScoped<ITransactionService, TransactionService>();
        services.AddScoped<IGeolocationApiService, GeolocationApiService>();
        services.AddScoped<IFileConversionService, FileConversionService>();
        
        services.Configure<TimeZoneApiOption>(Configuration.GetSection("TimeZoneApiOption"));
        services.Configure<DbConnection>(Configuration.GetSection("ConnectionStrings"));
        services.AddTransient<HttpClient>();
        
        services.AddControllers();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Transaction API",
                Version = "v1",
                Description = "API for transactions"
            });

            // XML-comment
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });
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
        
        // Enable Swagger in all environments
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "Transaction API V1");
        });

        app.UseRouting();
        app.UseCors(b => b.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
        
        app.UseEndpoints(endpoints =>
        {
            endpoints.MapControllers();
        });
    }

}