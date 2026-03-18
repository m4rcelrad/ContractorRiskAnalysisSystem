using CRAS.Domain.Interfaces;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

/// <summary>
///     Represents a report section that displays key financial ratios for a contractor.
/// </summary>
/// <remarks>
///     This section retrieves the most recent financial statement, delegates the mathematical
///     calculations to the injected <see cref="IRatioCalculator" />, and formats the results
///     into a structured three-column grid.
/// </remarks>
/// <param name="ratioCalculator">The domain service responsible for calculating financial ratios from a statement.</param>
public class FinancialRatiosSection(IRatioCalculator ratioCalculator) : IReportSection
{
    /// <summary>
    ///     Composes the visual layout for the financial ratios section within the PDF document.
    /// </summary>
    /// <param name="column">The column descriptor used to arrange elements vertically.</param>
    /// <param name="context">The context containing contractor data, risk assessments, and styling definitions.</param>
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

            table.Cell().RenderMetricCell("Current Ratio", ratios.CurrentRatio.ToString("F2"), context.Style);
            table.Cell().RenderMetricCell("Debt Ratio", ratios.DebtRatio.ToString("P1"), context.Style);
            table.Cell().RenderMetricCell("Return on Equity", ratios.ReturnOnEquity.ToString("P1"), context.Style);
            table.Cell().RenderMetricCell("Working Capital / Assets", ratios.WorkingCapitalToAssets.ToString("P1"), context.Style);
            table.Cell().RenderMetricCell("Asset Turnover", ratios.AssetTurnover.ToString("F2"), context.Style);
            table.Cell().RenderMetricCell("EBIT Margin", ratios.EbitMargin.ToString("P1"), context.Style);
        });
    }
}
