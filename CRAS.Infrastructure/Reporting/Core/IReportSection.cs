using QuestPDF.Fluent;

namespace CRAS.Infrastructure.Reporting.Core;

/// <summary>
/// Represents a section of a report, defining its structure and content.
/// </summary>
public interface IReportSection
{
    /// <summary>
    /// Defines the structure for composing sections of a report.
    /// </summary>
    /// <param name="columnDescriptor">Defines the layout and arrangement of elements within the column.</param>
    /// <param name="context">Provides contextual data and styling information necessary for rendering the report section.</param>
    void Compose(ColumnDescriptor columnDescriptor, ReportContext context);
}
