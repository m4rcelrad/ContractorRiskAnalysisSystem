using CRAS.Domain.Interfaces;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

public class PaymentBehaviorSection(IPaymentAnalyzer paymentAnalyzer) : IReportSection
{
    public void Compose(ColumnDescriptor column, ReportContext context)
    {
        column.Item().Text("Payment Behavior").Style(context.Style.SubHeaderStyle);

        if (context.Contractor.Invoices.Count == 0)
        {
            column.Item()
                .Text("No payment history available.")
                .Style(context.Style.MutedTextStyle);
            return;
        }

        var metrics = paymentAnalyzer.Analyze(context.Contractor.Invoices.ToList());

        column.Item().Table(table =>
        {
            table.ColumnsDefinition(columns =>
            {
                columns.RelativeColumn();
                columns.RelativeColumn();
                columns.RelativeColumn();
            });

            var delayText = $"{Math.Max(0, Math.Round(metrics.AverageDelayDays, 1))} days";
            var unpaidText = $"{metrics.UnpaidCount} ({metrics.UnpaidRatio:P1})";
            var amountText = metrics.TotalUnpaidAmount.ToString("N0");

            table.Cell().RenderMetricCell("Average Delay", delayText, context.Style);
            table.Cell().RenderMetricCell("Unpaid Invoices", unpaidText, context.Style);
            table.Cell().RenderMetricCell("Total Unpaid Amount", amountText, context.Style);
        });
    }
}
