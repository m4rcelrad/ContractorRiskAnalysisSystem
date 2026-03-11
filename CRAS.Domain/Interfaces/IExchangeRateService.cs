namespace CRAS.Domain.Interfaces;

/// <summary>
///     Defines the contract for a service that provides exchange rates.
/// </summary>
public interface IExchangeRateService
{
    Task<decimal> GetExchangeRateAsync(string currencyCode, DateTime date);
}