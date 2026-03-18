using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

/// <summary>
///     Represents a report section that displays the detailed results of various risk assessment models.
/// </summary>
/// <remarks>
///     This section generates a table listing each risk model used in the evaluation,
///     including its calculated numerical score and its corresponding risk level.
///     The risk levels are dynamically colored based on the severity defined in the style provider.
/// </remarks>
public class RiskAssessmentSection : IReportSection
{
    /// <summary>
    ///     Composes the visual layout for the risk assessment results section within the PDF document.
    /// </summary>
    /// <param name="column">The column descriptor used to arrange elements vertically.</param>
    /// <param name="context">The context containing contractor data, aggregated risk results, and styling definitions.</param>
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
