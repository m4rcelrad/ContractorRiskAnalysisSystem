using CRAS.Domain.Entities;
using CRAS.Domain.ValueObjects;
using CRAS.Infrastructure.Reporting;
using CRAS.Infrastructure.Reporting.Core;
using CRAS.Infrastructure.Reporting.Helpers;
using Moq;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Infrastructure;

namespace CRAS.Tests.Infrastructure.Reporting;

/// <summary>
///     Unit tests for the <see cref="ReportGenerator" /> class to ensure the PDF generation
///     process orchestrates sections correctly and produces a valid document.
/// </summary>
public class ReportGeneratorTests
{
    public ReportGeneratorTests()
    {
        Settings.License = LicenseType.Community;
    }

    /// <summary>
    ///     Verifies that the generator successfully produces a non-empty byte array
    ///     representing the PDF document when provided with valid data.
    /// </summary>
    [Fact]
    public void Generate_ShouldReturnNonEmptyByteArray_WhenGivenValidData()
    {
        var contractor = new Contractor { TaxId = "7740001454" };
        var assessment = new AggregatedRiskResult();
        var styleProvider = new StyleProvider();

        var mockSection = new Mock<IReportSection>();
        mockSection.Setup(s => s.Compose(It.IsAny<ColumnDescriptor>(), It.IsAny<ReportContext>()))
            .Callback<ColumnDescriptor, ReportContext>((col, _) => { col.Item().Text("Test Output"); });

        var generator = new ReportGenerator([ mockSection.Object ], styleProvider);

        var result = generator.Generate(contractor, assessment);

        Assert.NotNull(result);
        Assert.True(result.Length > 0);
    }

    /// <summary>
    ///     Verifies that the generator iterates through all injected sections and invokes
    ///     their Compose method to build the complete document.
    /// </summary>
    [Fact]
    public void Generate_ShouldInvokeComposeOnAllProvidedSections()
    {
        var contractor = new Contractor { TaxId = "7740001454" };
        var assessment = new AggregatedRiskResult();
        var styleProvider = new StyleProvider();

        var section1 = new Mock<IReportSection>();
        section1.Setup(s => s.Compose(It.IsAny<ColumnDescriptor>(), It.IsAny<ReportContext>()))
            .Callback<ColumnDescriptor, ReportContext>((col, _) => col.Item().Text("S1"));

        var section2 = new Mock<IReportSection>();
        section2.Setup(s => s.Compose(It.IsAny<ColumnDescriptor>(), It.IsAny<ReportContext>()))
            .Callback<ColumnDescriptor, ReportContext>((col, _) => col.Item().Text("S2"));

        var generator = new ReportGenerator([ section1.Object, section2.Object ], styleProvider);

        generator.Generate(contractor, assessment);

        section1.Verify(s => s.Compose(It.IsAny<ColumnDescriptor>(), It.IsAny<ReportContext>()), Times.Once);
        section2.Verify(s => s.Compose(It.IsAny<ColumnDescriptor>(), It.IsAny<ReportContext>()), Times.Once);
    }
}
