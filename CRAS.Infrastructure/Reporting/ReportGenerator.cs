using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.ValueObjects;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting;

/// <summary>
///     Orchestrates the generation of PDF reports by composing various report sections into a single document.
/// </summary>
/// <param name="sections">The collection of report sections to be rendered.</param>
/// <param name="styleProvider">The provider for document-wide styling and themes.</param>
public class ReportGenerator(IEnumerable<IReportSection> sections, IStyleProvider styleProvider) : IReportGenerator
{
    /// <summary>
    ///     Generates a PDF document for a specific contractor based on the provided risk assessment results.
    /// </summary>
    /// <param name="contractor">The contractor entity data to include in the report.</param>
    /// <param name="assessment">The aggregated risk results used to populate assessment sections.</param>
    /// <returns>A byte array representing the generated PDF report.</returns>
    public byte[] Generate(Contractor contractor, AggregatedRiskResult assessment)
    {
        var context = new ReportContext
        {
            Contractor = contractor,
            Assessment = assessment,
            Style = styleProvider
        };

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(context.Style.BaseStyle);

                page.Content().Column(col =>
                {
                    col.Spacing(20);

                    foreach (var section in sections)
                    {
                        section.Compose(col, context);
                    }
                });

                page.Footer().AlignCenter().Text(x =>
                {
                    x.Span("Page ");
                    x.CurrentPageNumber();
                    x.Span(" of ");
                    x.TotalPages();
                });
            });
        }).GeneratePdf();
    }
}
