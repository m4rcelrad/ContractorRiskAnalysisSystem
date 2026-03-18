using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

public class RiskAssessmentSection : IReportSection
{
    public void Compose(ColumnDescriptor column, ReportContext context)
    {
        column.Item().Text("Risk Models Assessment").Style(context.Style.SubHeaderStyle);

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.ConstantColumn(80);
                columns.ConstantColumn(80);
            });

            table.Header(header =>
            {
                header.Cell().RenderHeader("Model", context.Style);
                header.Cell().RenderHeader("Score", context.Style, true);
                header.Cell().RenderHeader("Level", context.Style, true);
            });

            foreach (var result in context.Assessment.IndividualResults)
            {
                var riskColor = context.Style.GetRiskColor(result.RiskLevel);

                table.Cell().RenderData(result.Model, context.Style);
                table.Cell().RenderData(result.Score.ToString("F3"), context.Style, true);
                table.Cell().RenderColoredData(result.RiskLevel.ToString(), context.Style, riskColor, true);
            }
        });
    }
}
