using System.Text.Json;
using CRAS.Domain.Interfaces;

namespace CRAS.Infrastructure.Services;

/// <summary>
///     A service for fetching exchange rates from the National Bank of Poland (NBP).
/// </summary>
/// <remarks>
///     This service uses the NBP's REST API to fetch exchange rates for a specific currency on a specific date.
/// </remarks>
/// <param name="httpClient"></param>
public class NbpExchangeRateService(HttpClient httpClient) : IExchangeRateService
{
    /// <summary>
    ///     Serializer options for deserializing JSON responses from the NBP API.
    /// </summary>
    private static readonly JsonSerializerOptions SerializerOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    /// <summary>
    ///     Fetches the exchange rate for a specific currency on a specific date from the NBP API.
    /// </summary>
    /// <remarks>
    ///     If the currency code is "PLN", the exchange rate is always 1.
    /// </remarks>
    /// <param name="currencyCode"></param>
    /// <param name="date"></param>
    /// <returns></returns>
    public async Task<decimal> GetExchangeRateAsync(string currencyCode, DateTime date)
    {
        if (currencyCode.Equals("PLN", StringComparison.OrdinalIgnoreCase)) return 1m;

        var formattedDate = date.ToString("yyyy-MM-dd");
        var url = $"https://api.nbp.pl/api/exchangerates/rates/a/{currencyCode}/{formattedDate}/?format=json";

        try
        {
            var response = await httpClient.GetAsync(url);

            if (!response.IsSuccessStatusCode) return 1m;

            var content = await response.Content.ReadAsStringAsync();
            var nbpResponse = JsonSerializer.Deserialize<NbpResponse>(content, SerializerOptions);

            return nbpResponse?.Rates?.FirstOrDefault()?.Mid ?? 1m;
        }
        catch
        {
            return 1m;
        }
    }

    /// <summary>
    ///     Class representing the JSON response from the NBP API.
    /// </summary>
    private class NbpResponse
    {
        public List<NbpRate> Rates { get; init; } = [];
    }

    /// <summary>
    ///     Class representing a single exchange rate from the NBP API.
    /// </summary>
    private class NbpRate
    {
        public decimal Mid { get; }
    }
}