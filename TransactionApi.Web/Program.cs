using Microsoft.EntityFrameworkCore;
using TransactionApi.Model;

namespace TransactionApi.Web;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = CreateHostBuilder(args);
        var host = builder.Build();
        
        // Выполняем миграции при старте
        using (var scope = host.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TransactionDbContext>();
            try
            {
                Console.WriteLine("Starting database migration...");
                context.Database.Migrate();
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"Error during migration: {ex.Message}");
            }
        }
        
        host.Run();
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, logging) =>
            {
                logging.AddDebug();
                logging.AddConsole();
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); });
}