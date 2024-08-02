using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Hosting;

namespace IntegrationTests.Factories;

public class CustomWebApplicationFactory : WebApplicationFactory<TestStartUp>
{
    protected override IHostBuilder CreateHostBuilder()
    {
        return Host.CreateDefaultBuilder()
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<TestStartUp>(); });
    }
}