using CRAS.Infrastructure.Reporting.Core;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

/// <summary>
/// Represents the header section of the financial report.
/// </summary>
public class HeaderSection : IReportSection
{
    public void Compose(ColumnDescriptor columnDescriptor, ReportContext context)
    {
        columnDescriptor.Item().Row(row =>
        {
            row.RelativeItem().Column(inner =>
            {
                inner.Item()
                    .Text("Contractor Financial Report")
                    .Style(context.Style.HeaderStyle);

                inner.Item()
                    .Text($"Tax ID: {context.Contractor.TaxId}")
                    .Style(context.Style.SubHeaderStyle)
                    .FontColor(context.Style.TextMutedColor);
            });

            row.ConstantItem(150).AlignRight().Column(inner =>
            {
                var riskColor = context.Style.GetRiskColor(context.Assessment.OverallRiskLevel);

                inner.Item()
                    .Text("Overall Status")
                    .Style(context.Style.LabelStyle)
                    .AlignRight();

                inner.Item()
                    .Text(context.Assessment.OverallRiskLevel.ToString().ToUpper())
                    .Style(context.Style.ValueStyle)
                    .FontSize(16)
                    .FontColor(riskColor)
                    .AlignRight();
            });
        });

        columnDescriptor.Item()
            .PaddingVertical(10)
            .LineHorizontal(1)
            .LineColor(context.Style.BorderColor);
    }
}
