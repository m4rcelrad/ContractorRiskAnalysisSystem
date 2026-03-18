using CRAS.Domain.Enums;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting.Helpers;

/// <summary>
///     Represents a service that provides styles for the report.
/// </summary>
public interface IStyleProvider
{
    string BorderColor { get; }
    string TextMutedColor { get; }

    TextStyle BaseStyle { get; }
    TextStyle HeaderStyle { get; }
    TextStyle SubHeaderStyle { get; }
    TextStyle LabelStyle { get; }
    TextStyle ValueStyle { get; }
    TextStyle MutedTextStyle { get; }
    string GetRiskColor(RiskLevel riskLevel);
}
