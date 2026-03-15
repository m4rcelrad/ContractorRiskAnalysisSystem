using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using CRAS.Application.Models;

namespace CRAS.Web.Services;

public class DashboardStateService
{
    private readonly HttpClient _http;
    private readonly JsonSerializerOptions _options;

    public List<DashboardOverviewResponse>? Overviews { get; private set; }
    public DateTime LastFetch { get; private set; }

    public DashboardStateService(HttpClient http)
    {
        _http = http;
        _options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter() }
        };
    }

    public async Task LoadDataAsync(bool forceRefresh = false)
    {
        if (Overviews != null && !forceRefresh && (DateTime.UtcNow - LastFetch).TotalMinutes < 5)
        {
            return;
        }

        Overviews = await _http.GetFromJsonAsync<List<DashboardOverviewResponse>>("api/Contractors/dashboard", _options);
        LastFetch = DateTime.UtcNow;
    }
}
