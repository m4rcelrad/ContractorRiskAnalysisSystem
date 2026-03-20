using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CRAS.Application.Models;

namespace CRAS.Web.Services;

/// <summary>
///     Manages the frontend state for the risk dashboard.
///     Provides in-memory caching and resilient data fetching from the API.
/// </summary>
/// <param name="http">The HTTP client used to communicate with the backend API.</param>
public class DashboardStateService(HttpClient http, IConfiguration configuration)
{
    /// <summary>
    ///     JSON serialization options configured to handle enums as strings and be case-insensitive.
    /// </summary>
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    /// <summary>
    ///     Gets the cached list of contractor dashboard overviews.
    ///     Returns null if data has not been fetched yet.
    /// </summary>
    public List<DashboardOverviewResponse>? Overviews { get; private set; }

    /// <summary>
    ///     Gets or sets the timestamp of the last successful data fetch.
    /// </summary>
    private DateTime LastFetch { get; set; }

    /// <summary>
    ///     Fetches dashboard data from the API and updates the local state.
    ///     Implements a 5-minute Time-To-Live (TTL) cache and a basic retry policy to handle startup race conditions.
    /// </summary>
    /// <param name="forceRefresh">If true, bypasses the local cache and forces a new API request.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public async Task LoadDataAsync(bool forceRefresh = false)
    {
        if (Overviews != null && !forceRefresh && (DateTime.UtcNow - LastFetch).TotalMinutes < 5)
        {
            return;
        }

        var endpoint = configuration["ApiSettings:DashboardEndpoint"] ?? "api/Contractors/dashboard";

        const int maxRetries = 3;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                Overviews = await http.GetFromJsonAsync<List<DashboardOverviewResponse>>(endpoint, _options);
                LastFetch = DateTime.UtcNow;
                return;
            }
            catch (HttpRequestException) when (i < maxRetries - 1)
            {
                await Task.Delay(1000);
            }
        }
    }
}
