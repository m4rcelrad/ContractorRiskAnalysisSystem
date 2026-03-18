using CRAS.Domain.Enums;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

public class FinancialHistorySection : IReportSection
{
    public void Compose(ColumnDescriptor column, ReportContext context)
    {
        column.Item().Text("Financial Statement History").Style(context.Style.SubHeaderStyle);

        if (context.Contractor.FinancialStatements.Count == 0)
        {
            column.Item().Text("No financial history available.").Style(context.Style.MutedTextStyle);
            return;
        }

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.ConstantColumn(40);
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            table.Header(header =>
            {
                header.Cell().RenderHeader("Year", context.Style);
                header.Cell().RenderHeader("Assets", context.Style, true);
                header.Cell().RenderHeader("Liabilities", context.Style, true);
                header.Cell().RenderHeader("Working\nCapital", context.Style, true);
                header.Cell().RenderHeader("EBIT", context.Style, true);
                header.Cell().RenderHeader("Sales", context.Style, true);
                header.Cell().RenderHeader("Net Income", context.Style, true);
            });

            foreach (var statement in context.Contractor.FinancialStatements.OrderByDescending(s => s.Year))
            {
                table.Cell().RenderData(statement.Year.ToString(), context.Style);
                table.Cell().RenderData(statement.TotalAssets.ToString("N0"), context.Style, true);
                table.Cell().RenderData(statement.TotalLiabilities.ToString("N0"), context.Style, true);
                table.Cell().RenderData(statement.WorkingCapital.ToString("N0"), context.Style, true);
                table.Cell().RenderData(statement.EBIT.ToString("N0"), context.Style, true);
                table.Cell().RenderData(statement.Sales.ToString("N0"), context.Style, true);

                var incomeColor = statement.NetIncome >= 0
                    ? context.Style.GetRiskColor(RiskLevel.Low)
                    : context.Style.GetRiskColor(RiskLevel.Critical);

                table.Cell().RenderColoredData(statement.NetIncome.ToString("N0"), context.Style, incomeColor, true);
            }
        });
    }
}
