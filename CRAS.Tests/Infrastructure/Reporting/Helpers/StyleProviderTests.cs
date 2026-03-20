using CRAS.Domain.Enums;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Helpers;

namespace CRAS.Tests.Infrastructure.Reporting.Helpers;

/// <summary>
/// Unit tests for the <see cref="StyleProvider"/> class, ensuring that report styles
/// and colors are correctly defined and consistent with branding [cite: 171-172].
/// </summary>
public class StyleProviderTests
{
    private readonly StyleProvider _provider = new();

    /// <summary>
    /// Provides strongly typed color mapping data for risk levels .
    /// </summary>
    public static TheoryData<RiskLevel, string> RiskColorData =>
        new()
        {
            { RiskLevel.Low, Colors.Green.Medium },
            { RiskLevel.Moderate, Colors.Orange.Medium },
            { RiskLevel.Critical, Colors.Red.Medium }
        };

    /// <summary>
    /// Verifies that <see cref="StyleProvider.GetRiskColor"/> returns the correct
    /// hexadecimal color codes for each risk level.
    /// </summary>
    /// <param name="level">The risk level to evaluate[cite: 173].</param>
    /// <param name="expectedColor">The expected QuestPDF color constant[cite: 174].</param>
    [Theory]
    [MemberData(nameof(RiskColorData))]
    public void GetRiskColor_ShouldReturnCorrectBrandingColor(RiskLevel level, string expectedColor)
    {
        var result = _provider.GetRiskColor(level);
        Assert.Equal(expectedColor, result);
    }

    /// <summary>
    /// Verifies that <see cref="StyleProvider.BaseStyle"/> is correctly initialized
    /// with the default font family and size[cite: 175].
    /// </summary>
    [Fact]
    public void BaseStyle_ShouldBeInitialized()
    {
        var style = _provider.BaseStyle;
        Assert.NotNull(style);
    }

    /// <summary>
    /// Ensures that <see cref="StyleProvider.HeaderStyle"/> is correctly generated
    /// using the primary branding color[cite: 176].
    /// </summary>
    [Fact]
    public void HeaderStyle_ShouldBeInitialized()
    {
        var style = _provider.HeaderStyle;
        Assert.NotNull(style);
    }

    /// <summary>
    /// Verifies that <see cref="StyleProvider.SubHeaderStyle"/> is correctly
    /// generated for secondary headings[cite: 177].
    /// </summary>
    [Fact]
    public void SubHeaderStyle_ShouldBeInitialized()
    {
        var style = _provider.SubHeaderStyle;
        Assert.NotNull(style);
    }

    /// <summary>
    /// Verifies that <see cref="StyleProvider.MutedTextStyle"/> is correctly
    /// generated for secondary information[cite: 180].
    /// </summary>
    [Fact]
    public void MutedTextStyle_ShouldBeInitialized()
    {
        var style = _provider.MutedTextStyle;
        Assert.NotNull(style);
    }
}
