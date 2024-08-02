using DiffAPI.Interfaces;
using DiffAPI.Services;

namespace IntegrationTests;

public class TestStartUp
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddSingleton<IDiffService, DiffService>();
    }

    public static void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseRouting();
        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }
}