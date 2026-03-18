using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting;

public class ReportGenerator(IEnumerable<IReportSection> sections, IStyleProvider styleProvider) : IReportGenerator
{
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
