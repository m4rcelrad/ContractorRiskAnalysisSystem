using System.Reflection;
using System.Text.Json.Serialization;
using CRAS.Api.Filters;
using CRAS.Application.Requests;
using CRAS.Application.Validators;
using CRAS.Domain.Interfaces;
using CRAS.Domain.RiskModels;
using CRAS.Domain.Services;
using CRAS.Domain.Strategies;
using CRAS.Infrastructure.Data;
using CRAS.Infrastructure.Reporting;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using CRAS.Infrastructure.Reporting.Sections;
using CRAS.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using QuestPDF;
using QuestPDF.Infrastructure;

namespace CRAS.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers(options => { options.Filters.Add<ValidationFilter>(); }).AddJsonOptions(options =>
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

        builder.Services.AddScoped<IValidator<AddContractorRequest>, AddContractRequestValidator>();
        builder.Services.AddScoped<IValidator<AddInvoiceRequest>, AddInvoiceRequestValidator>();
        builder.Services.AddScoped<IValidator<AddFinancialStatementRequest>, AddFinancialStatementRequestValidator>();

        builder.Services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
            {
                policy.WithOrigins(
                        "http://localhost:5265",
                        "http://localhost:5000",
                        "https://localhost:5001",
                        "http://localhost:5250")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddOutputCache();

        Settings.License = LicenseType.Community;

        builder.Services.AddScoped<IReportGenerator, ReportGenerator>();
        builder.Services.AddSingleton<IStyleProvider, StyleProvider>();

        builder.Services.AddScoped<IReportSection, HeaderSection>();
        builder.Services.AddScoped<IReportSection, RiskAssessmentSection>();
        builder.Services.AddScoped<IReportSection, FinancialRatiosSection>();
        builder.Services.AddScoped<IReportSection, PaymentBehaviorSection>();
        builder.Services.AddScoped<IReportSection, FinancialHistorySection>();

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
