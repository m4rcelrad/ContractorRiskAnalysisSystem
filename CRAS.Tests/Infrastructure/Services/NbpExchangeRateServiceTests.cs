using System.Net;
using CRAS.Infrastructure.Services;
using Moq;
using Moq.Protected;

namespace CRAS.Tests.Infrastructure.Services;

/// <summary>
/// Unit tests for the <see cref="NbpExchangeRateService"/> verify API interaction and fallback logic.
/// </summary>
public class NbpExchangeRateServiceTests
{
    /// <summary>
    /// Ensures that the service returns a fixed rate of 1.0 for the local currency (PLN)
    /// without making any external API calls[cite: 247].
    /// </summary>
    [Fact]
    public async Task GetExchangeRateAsync_ReturnsOne_ForPln()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        var httpClient = new HttpClient(handlerMock.Object);
        var service = new NbpExchangeRateService(httpClient);

        var result = await service.GetExchangeRateAsync("PLN", DateTime.UtcNow);

        Assert.Equal(1m, result);
        handlerMock.Protected().Verify(
            "SendAsync",
            Times.Never(),
            ItExpr.IsAny<HttpRequestMessage>(),
            ItExpr.IsAny<CancellationToken>());
    }

    /// <summary>
    /// Verifies that the service handles API failures (e.g., 404 Not Found) by returning
    /// a safe default exchange rate of 1.0[cite: 249, 251].
    /// </summary>
    [Fact]
    public async Task GetExchangeRateAsync_ReturnsOne_OnApiError()
    {
        var handlerMock = new Mock<HttpMessageHandler>();
        handlerMock.Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>())
            .ReturnsAsync(new HttpResponseMessage(HttpStatusCode.NotFound));

        var httpClient = new HttpClient(handlerMock.Object);
        var service = new NbpExchangeRateService(httpClient);

        var result = await service.GetExchangeRateAsync("USD", DateTime.UtcNow);

        Assert.Equal(1m, result);
    }
}
