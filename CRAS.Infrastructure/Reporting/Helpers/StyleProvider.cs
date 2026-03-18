using CRAS.Domain.Enums;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting.Helpers;

public class StyleProvider : IStyleProvider
{
    public string GetRiskColor(RiskLevel level)
    {
        return level switch
        {
            RiskLevel.Low => Colors.Green.Medium,
            RiskLevel.Moderate => Colors.Orange.Medium,
            RiskLevel.Critical => Colors.Red.Medium,
            _ => Colors.Grey.Medium
        };
    }

    public string PrimaryColor => Colors.Blue.Darken3;
    public string BorderColor => Colors.Grey.Lighten2;
    public string TextMutedColor => Colors.Grey.Medium;

    public TextStyle BaseStyle => TextStyle.Default
        .FontFamily(Fonts.SegoeUI)
        .FontSize(10)
        .FontColor(Colors.Black);

    public TextStyle HeaderStyle => BaseStyle
        .FontSize(24)
        .SemiBold()
        .FontColor(PrimaryColor);

    public TextStyle SubHeaderStyle => BaseStyle
        .FontSize(14)
        .SemiBold()
        .FontColor(Colors.Blue.Darken2);

    public TextStyle LabelStyle => BaseStyle
        .FontSize(8)
        .FontColor(Colors.Grey.Darken1)
        .SemiBold();

    public TextStyle ValueStyle => BaseStyle
        .FontSize(12)
        .SemiBold();

    public TextStyle MutedTextStyle => BaseStyle
        .FontSize(9)
        .FontColor(TextMutedColor)
        .Italic();
}
