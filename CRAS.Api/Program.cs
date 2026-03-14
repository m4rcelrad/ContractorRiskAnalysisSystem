using System.Reflection;
using CRAS.Application.Validators;
using CRAS.Domain.Engine;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using CRAS.Domain.Strategies;
using CRAS.Infrastructure.Data;
using CRAS.Infrastructure.Services;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Api;

public static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

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

        builder.Services.AddSwaggerGen(c =>
        {
            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            c.IncludeXmlComments(xmlPath);
        });

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

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
