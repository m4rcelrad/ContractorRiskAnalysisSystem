using CRAS.Domain.Interfaces;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Sections;

/// <summary>
///     Represents a report section that analyzes and displays the contractor's payment behavior.
/// </summary>
/// <remarks>
///     This section provides insights into the contractor's reliability by presenting metrics such as
///     average payment delays, the count and ratio of unpaid invoices, and the total outstanding debt.
///     It uses the <see cref="IPaymentAnalyzer" /> domain service to process raw invoice data.
/// </remarks>
/// <param name="paymentAnalyzer">The domain service used to calculate payment metrics from invoice history.</param>
public class PaymentBehaviorSection(IPaymentAnalyzer paymentAnalyzer) : IReportSection
{
    /// <summary>
    ///     Composes the visual layout for the payment behavior section within the PDF document.
    /// </summary>
    /// <param name="column">The column descriptor used to arrange elements vertically.</param>
    /// <param name="context">The context containing contractor data, risk assessments, and styling definitions.</param>
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
