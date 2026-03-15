using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CRAS.Application.Models;

namespace CRAS.Web.Services;

public class DashboardStateService(HttpClient http)
{
    private readonly JsonSerializerOptions _options = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public List<DashboardOverviewResponse>? Overviews { get; private set; }
    private DateTime LastFetch { get; set; }

    public async Task LoadDataAsync(bool forceRefresh = false)
    {
        if (Overviews != null && !forceRefresh && (DateTime.UtcNow - LastFetch).TotalMinutes < 5)
        {
            return;
        }

        const int maxRetries = 3;

        for (var i = 0; i < maxRetries; i++)
        {
            try
            {
                Overviews = await http.GetFromJsonAsync<List<DashboardOverviewResponse>>("api/Contractors/dashboard", _options);
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
