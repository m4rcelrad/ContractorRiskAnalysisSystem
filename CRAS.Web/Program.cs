using System.Text.Json;
using System.Text.Json.Serialization;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using CRAS.Web.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

namespace CRAS.Web;

/// <summary>
///     The main entry point for the CRAS WebAssembly application.
/// </summary>
public static class Program
{
    /// <summary>
    ///     Configures and builds the WebAssembly host, registers dependency injection services,
    ///     and starts the application.
    /// </summary>
    /// <param name="args">Command-line arguments passed to the application.</param>
    /// <returns>A task that represents the asynchronous run operation.</returns>
    public async static Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        builder.Services.AddScoped(_ => new HttpClient
        {
            BaseAddress = new Uri("http://localhost:5250/")
        });

        builder.Services.AddScoped<DashboardStateService>();

        builder.Services.AddScoped<IRatioCalculator, RatioCalculator>();
        builder.Services.AddScoped<IPaymentAnalyzer, PaymentAnalyzer>();

        builder.Services.Configure<JsonSerializerOptions>(options =>
        {
            options.Converters.Add(new JsonStringEnumConverter());
            options.PropertyNameCaseInsensitive = true;
        });

        await builder.Build().RunAsync();
    }
}
