using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting.Helpers;

internal static class TableBuilderExtensions
{
    public static void RenderHeader(this IContainer container, string text, IStyleProvider style, bool alignRight = false)
    {
        var cell = container
            .BorderBottom(1)
            .BorderColor(style.BorderColor)
            .PaddingBottom(5);

        if (alignRight) cell.AlignRight();

        cell.Text(text).Style(style.LabelStyle);
    }

    public static void RenderData(this IContainer container, string text, IStyleProvider style, bool alignRight = false)
    {
        var cell = container
            .PaddingVertical(4)
            .BorderBottom(1)
            .BorderColor(style.BorderColor);

        if (alignRight) cell.AlignRight();

        cell.Text(text).Style(style.BaseStyle).FontSize(9);
    }

    public static void RenderColoredData(this IContainer container, string text, IStyleProvider style, string color, bool alignRight = false)
    {
        var cell = container
            .PaddingVertical(4)
            .BorderBottom(1)
            .BorderColor(style.BorderColor);

        if (alignRight) cell.AlignRight();

        cell.Text(text)
            .Style(style.BaseStyle)
            .FontSize(9)
            .FontColor(color)
            .SemiBold();
    }

    public static void RenderMetricCell(this IContainer container, string label, string value, IStyleProvider style)
    {
        container.Padding(5).Column(col =>
        {
            col.Item().Text(label).Style(style.LabelStyle);
            col.Item().Text(value).Style(style.ValueStyle);
        });
    }
}
