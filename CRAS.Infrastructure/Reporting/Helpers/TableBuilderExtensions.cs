using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CRAS.Infrastructure.Reporting.Helpers;

internal static class TableBuilderExtensions
{
    extension(IContainer container)
    {
        public void RenderHeader(string text, IStyleProvider style, bool alignRight = false)
        {
            var cell = container
                .BorderBottom(1)
                .BorderColor(style.BorderColor)
                .PaddingBottom(5);

            if (alignRight)
                cell = cell.AlignRight();

            cell.Text(text).Style(style.LabelStyle);
        }

        public void RenderData(string text, IStyleProvider style, bool alignRight = false)
        {
            var cell = container
                .PaddingVertical(4)
                .BorderBottom(1)
                .BorderColor(style.BorderColor);

            if (alignRight)
                cell = cell.AlignRight();

            cell.Text(text).Style(style.BaseStyle).FontSize(9);
        }

        public void RenderColoredData(string text, IStyleProvider style, string color, bool alignRight = false)
        {
            var cell = container
                .PaddingVertical(4)
                .BorderBottom(1)
                .BorderColor(style.BorderColor);

            if (alignRight)
                cell = cell.AlignRight();

            cell.Text(text)
                .Style(style.BaseStyle)
                .FontSize(9)
                .FontColor(color)
                .SemiBold();
        }

        public void RenderMetricCell(string label, string value, IStyleProvider style)
        {
            container.Padding(5).Column(col =>
            {
                col.Item().Text(label).Style(style.LabelStyle);
                col.Item().Text(value).Style(style.ValueStyle);
            });
        }
    }
}
