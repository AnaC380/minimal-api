using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.Text;
using minimal_api.Dominio.Interfaces; 
using Test.Mocks;
using Microsoft.Extensions.DependencyInjection;

namespace Test.Helpers;

internal class Setup
{
    public const string PORT = "5001";
    public static TestContext TestContext = default!;
    public static WebApplicationFactory<Startup> http = default!;
    public static HttpClient Client = default!;

    public static void ClassInit(TestContext testContext)
    {
        Setup.TestContext = testContext;
        Setup.http = new WebApplicationFactory<Startup>().WithWebHostBuilder(builder =>
        {
            builder.UseSetting("https_port", Setup.PORT).UseEnvironment("Testing");

            builder.ConfigureServices(services =>
            {
                services.AddScoped<IAdministradorServicos, AdministradorServicoMocks>();
            });
        });

        Setup.Client = Setup.http.CreateClient();
    }

    public static void ClassCleanup()
    {
        Setup.http.Dispose();
    }
}   
