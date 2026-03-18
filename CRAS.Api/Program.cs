using System.Reflection;
using System.Text.Json.Serialization;
using CRAS.Application.Validators;
using CRAS.Domain.Engine;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using CRAS.Domain.Strategies;
using CRAS.Infrastructure.Data;
using CRAS.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Infrastructure;

namespace CRAS.Api;

/// <summary>
/// The entry point for the application.
/// </summary>
/// <remarks>
/// This class configures services, middleware, and other settings required
/// for the application at startup. It includes dependency injection setup,
/// database initialization, CORS policy definition, and API documentation
/// configurations. This class is automatically executed when the application
/// starts.
/// </remarks>
public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

        builder.Services.AddScoped<IRiskAggregationStrategy, MajorityVoteAggregation>();
        builder.Services.AddScoped<IRiskModel, AltmanZScoreModel>();
        builder.Services.AddScoped<IRiskModel, AltmanZDoublePrimeModel>();
        builder.Services.AddScoped<IRiskModel, OhlsonOScoreModel>();

        builder.Services.AddScoped<IBehavioralRiskModel, PaymentDelayModel>();

        builder.Services.AddScoped<IRiskEngine, RiskEngine>();

        builder.Services.AddHttpClient<IExchangeRateService, NbpExchangeRateService>();

        builder.Services.AddValidatorsFromAssemblyContaining<AddInvoiceRequestValidator>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5265",
                        "http://localhost:5000",
                        "https://localhost:5001")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddOutputCache();

        QuestPDF.Settings.License = LicenseType.Community;
        builder.Services.AddScoped<IReportGenerator, ReportGenerator>();

        builder.Services.AddScoped<IRatioCalculator, RatioCalculator>();
        builder.Services.AddScoped<IPaymentAnalyzer, PaymentAnalyzer>();

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            using var scope = app.Services.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            dbContext.Database.Migrate();
            DataSeeder.Seed(dbContext);
        }

        app.UseCors();
        app.UseOutputCache();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
