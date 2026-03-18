using CRAS.Domain.Enums;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting.Helpers;

public interface IStyleProvider
{
    string GetRiskColor(RiskLevel riskLevel);

    string PrimaryColor { get; }
    string BorderColor { get; }
    string TextMutedColor { get; }

    TextStyle BaseStyle { get; }
    TextStyle HeaderStyle { get; }
    TextStyle SubHeaderStyle { get; }
    TextStyle LabelStyle { get; }
    TextStyle ValueStyle { get; }
    TextStyle MutedTextStyle { get; }
}
