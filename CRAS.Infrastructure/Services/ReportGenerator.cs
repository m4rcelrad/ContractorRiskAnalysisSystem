using CRAS.Domain.Entities;
using CRAS.Domain.Enums;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Models;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Services;

public class ReportGenerator : IReportGenerator
{
    public byte[] Generate(Contractor contractor, AggregatedRiskResult assessment)
    {
        var latestStatement = contractor.FinancialStatements.OrderByDescending(s => s.Year).FirstOrDefault();
        var unpaidInvoices = contractor.Invoices.Where(i => !i.IsPaid).ToList();
        var paidInvoices = contractor.Invoices.Where(i => i.IsPaid).ToList();

        var totalUnpaidAmount = unpaidInvoices.Sum(i => i.Amount);
        var avgDelay = paidInvoices.Count != 0 ? paidInvoices.Average(i => (i.PaymentDate!.Value.Date - i.DueDate.Date).TotalDays) : 0;
        var unpaidRatio = contractor.Invoices.Count > 0 ? (decimal)unpaidInvoices.Count / contractor.Invoices.Count : 0m;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(1.5f, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.SegoeUI));

                page.Header().Column(col =>
                {
                    col.Item().Row(row =>
                    {
                        row.RelativeItem().Column(inner =>
                        {
                            inner.Item().Text("Contractor Financial Report").FontSize(24).SemiBold().FontColor(Colors.Blue.Darken3);
                            inner.Item().Text($"Tax ID: {contractor.TaxId}").FontSize(14).FontColor(Colors.Grey.Darken2);
                        });
                        row.ConstantItem(150).AlignRight().Column(inner =>
                        {
                            var riskColor = assessment.OverallRiskLevel switch
                            {
                                RiskLevel.Low => Colors.Green.Medium,
                                RiskLevel.Moderate => Colors.Orange.Medium,
                                RiskLevel.Critical => Colors.Red.Medium,
                                _ => Colors.Grey.Medium
                            };

                            inner.Item().Text("Overall Status").FontSize(10).FontColor(Colors.Grey.Medium).AlignRight();
                            inner.Item().Text(assessment.OverallRiskLevel.ToString().ToUpper()).FontSize(16).SemiBold().FontColor(riskColor)
                                .AlignRight();
                        });
                    });
                    col.Item().PaddingVertical(10).LineHorizontal(1).LineColor(Colors.Grey.Lighten2);
                });

                page.Content().Column(col =>
                {
                    col.Spacing(20);

                    col.Item().Text("Risk Models Assessment").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.ConstantColumn(80);
                            columns.ConstantColumn(80);
                        });
                        table.Header(header =>
                        {
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Model").SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Score")
                                .SemiBold();
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Level")
                                .SemiBold();
                        });

                        foreach (var result in assessment.IndividualResults)
                        {
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Text(result.Model);
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(result.Score.ToString("F3"));
                            table.Cell().PaddingVertical(5).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(result.RiskLevel.ToString());
                        }
                    });

                    if (latestStatement != null)
                    {
                        col.Item().Text($"Key Financial Ratios ({latestStatement.Year})").FontSize(14).SemiBold()
                            .FontColor(Colors.Blue.Darken2);
                        col.Item().Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                                columns.RelativeColumn();
                            });

                            RenderRatioCell(table, "Current Ratio",
                                CalculateRatio(latestStatement.CurrentAssets, latestStatement.CurrentLiabilities).ToString("F2"));
                            RenderRatioCell(table, "Debt Ratio",
                                CalculateRatio(latestStatement.TotalLiabilities, latestStatement.TotalAssets).ToString("P1"));
                            RenderRatioCell(table, "Return on Equity",
                                CalculateRatio(latestStatement.NetIncome, latestStatement.BookValueEquity).ToString("P1"));
                            RenderRatioCell(table, "Working Capital / Assets",
                                CalculateRatio(latestStatement.WorkingCapital, latestStatement.TotalAssets).ToString("P1"));
                            RenderRatioCell(table, "Asset Turnover",
                                CalculateRatio(latestStatement.Sales, latestStatement.TotalAssets).ToString("F2"));
                            RenderRatioCell(table, "EBIT Margin", CalculateRatio(latestStatement.EBIT, latestStatement.Sales).ToString("P1"));
                        });
                    }

                    col.Item().Text("Payment Behavior").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        var delayText = $"{Math.Max(0, Math.Round(avgDelay, 1))} days";
                        var unpaidText = $"{unpaidInvoices.Count} ({unpaidRatio:P1})";
                        var amountText = totalUnpaidAmount.ToString("N0");

                        RenderRatioCell(table, "Average Delay", delayText);
                        RenderRatioCell(table, "Unpaid Invoices", unpaidText);
                        RenderRatioCell(table, "Total Unpaid Amount", amountText);
                    });

                    col.Item().Text("Financial Statement History").FontSize(14).SemiBold().FontColor(Colors.Blue.Darken2);
                    col.Item().Table(table =>
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
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).Text("Year").SemiBold()
                                .FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Assets")
                                .SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Liabilities")
                                .SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Working Cap")
                                .SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("EBIT")
                                .SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Sales")
                                .SemiBold().FontSize(9);
                            header.Cell().BorderBottom(1).BorderColor(Colors.Grey.Lighten2).PaddingBottom(5).AlignRight().Text("Net Income")
                                .SemiBold().FontSize(9);
                        });

                        foreach (var statement in contractor.FinancialStatements.OrderByDescending(s => s.Year))
                        {
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).Text(statement.Year.ToString())
                                .FontSize(9);
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(statement.TotalAssets.ToString("N0")).FontSize(9);
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(statement.TotalLiabilities.ToString("N0")).FontSize(9);
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(statement.WorkingCapital.ToString("N0")).FontSize(9);
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(statement.EBIT.ToString("N0")).FontSize(9);
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(statement.Sales.ToString("N0")).FontSize(9);
                            table.Cell().PaddingVertical(4).BorderBottom(1).BorderColor(Colors.Grey.Lighten3).AlignRight()
                                .Text(statement.NetIncome.ToString("N0")).FontSize(9);
                        }
                    });
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

    private static void RenderRatioCell(TableDescriptor table, string label, string value)
    {
        table.Cell().Padding(5).Column(col =>
        {
            col.Item().Text(label).FontSize(8).FontColor(Colors.Grey.Darken1).SemiBold();
            col.Item().Text(value).FontSize(12).SemiBold();
        });
    }

    private static decimal CalculateRatio(decimal numerator, decimal denominator)
    {
        if (denominator == 0) return 0m;
        return numerator / denominator;
    }
}
