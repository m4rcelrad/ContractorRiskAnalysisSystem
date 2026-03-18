using System.Linq;
using CRAS.Domain.Interfaces;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

public class FinancialRatiosSection(IRatioCalculator ratioCalculator) : IReportSection
{
    public void Compose(ColumnDescriptor column, ReportContext context)
    {
        var latestStatement = context.Contractor.FinancialStatements
            .OrderByDescending(s => s.Year)
            .FirstOrDefault();

        if (latestStatement == null) return;

        var ratios = ratioCalculator.CalculateFor(latestStatement);

        column.Item().Text($"Key Financial Ratios ({latestStatement.Year})")
            .Style(context.Style.SubHeaderStyle);

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            // Używamy naszego globalnego helpera RenderMetricCell
            table.Cell().RenderMetricCell("Current Ratio", ratios.CurrentRatio.ToString("F2"), context.Style);
            table.Cell().RenderMetricCell("Debt Ratio", ratios.DebtRatio.ToString("P1"), context.Style);
            table.Cell().RenderMetricCell("Return on Equity", ratios.ReturnOnEquity.ToString("P1"), context.Style);
            table.Cell().RenderMetricCell("Working Capital / Assets", ratios.WorkingCapitalToAssets.ToString("P1"), context.Style);
            table.Cell().RenderMetricCell("Asset Turnover", ratios.AssetTurnover.ToString("F2"), context.Style);
            table.Cell().RenderMetricCell("EBIT Margin", ratios.EbitMargin.ToString("P1"), context.Style);
        });
    }
}
