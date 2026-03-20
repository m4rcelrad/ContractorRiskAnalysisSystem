using CRAS.Domain.Entities;
using CRAS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace CRAS.Tests.Infrastructure;

/// <summary>
/// Unit tests verifying the Entity Framework Core model configurations, ensuring
/// database constraints and relationships are correctly applied.
/// </summary>
public class EntityConfigurationsTests
{
    private readonly IModel _model;

    public EntityConfigurationsTests()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql("Host=dummy;Database=dummy;Username=dummy;Password=dummy")
            .Options;

        using var context = new AppDbContext(options);
        _model = context.Model;
    }

    /// <summary>
    /// Verifies that the <see cref="Contractor"/> entity has the correct database
    /// limitations, specifically the maximum length and requirement of the TaxId property.
    /// </summary>
    [Fact]
    public void ContractorConfiguration_ShouldApplyCorrectConstraints()
    {
        var entityType = _model.FindEntityType(typeof(Contractor));
        Assert.NotNull(entityType);

        var taxIdProperty = entityType.FindProperty(nameof(Contractor.TaxId));
        Assert.NotNull(taxIdProperty);
        Assert.Equal(10, taxIdProperty.GetMaxLength());
        Assert.False(taxIdProperty.IsNullable);
    }

    /// <summary>
    /// Verifies that the <see cref="FinancialStatement"/> entity applies correct PostgreSQL
    /// numeric types to precision-sensitive financial data to prevent rounding errors.
    /// </summary>
    [Fact]
    public void FinancialStatementConfiguration_ShouldApplyCorrectNumericTypes()
    {
        var entityType = _model.FindEntityType(typeof(FinancialStatement));
        Assert.NotNull(entityType);

        var totalAssetsProperty = entityType.FindProperty(nameof(FinancialStatement.TotalAssets));
        Assert.NotNull(totalAssetsProperty);
        Assert.Equal("numeric(18,2)", totalAssetsProperty.GetColumnType());

        var gnpPriceIndexProperty = entityType.FindProperty(nameof(FinancialStatement.GNPPriceIndex));
        Assert.NotNull(gnpPriceIndexProperty);
        Assert.Equal("numeric(18,4)", gnpPriceIndexProperty.GetColumnType());
    }

    /// <summary>
    /// Verifies that the <see cref="Invoice"/> entity configurations are applied,
    /// including ignoring dynamically calculated properties and limiting currency code length.
    /// </summary>
    [Fact]
    public void InvoiceConfiguration_ShouldApplyCorrectConstraintsAndIgnoreProperties()
    {
        var entityType = _model.FindEntityType(typeof(Invoice));
        Assert.NotNull(entityType);

        var currencyProperty = entityType.FindProperty(nameof(Invoice.Currency));
        Assert.NotNull(currencyProperty);
        Assert.Equal(3, currencyProperty.GetMaxLength());

        var amountProperty = entityType.FindProperty(nameof(Invoice.Amount));
        Assert.NotNull(amountProperty);
        Assert.Equal("numeric(18,2)", amountProperty.GetColumnType());

        var delayInDaysProperty = entityType.FindProperty(nameof(Invoice.DelayInDays));
        Assert.Null(delayInDaysProperty);
    }

    /// <summary>
    /// Verifies that the delete behaviors between entities are configured correctly,
    /// ensuring cascade deletion from Contractors to related tables.
    /// </summary>
    [Fact]
    public void Relationships_ShouldHaveCascadeDeleteBehavior()
    {
        var contractorType = _model.FindEntityType(typeof(Contractor));
        Assert.NotNull(contractorType);

        var statementsForeignKey = contractorType.GetReferencingForeignKeys()
            .FirstOrDefault(fk => fk.DeclaringEntityType.ClrType == typeof(FinancialStatement));

        Assert.NotNull(statementsForeignKey);
        Assert.Equal(DeleteBehavior.Cascade, statementsForeignKey.DeleteBehavior);

        var invoicesForeignKey = contractorType.GetReferencingForeignKeys()
            .FirstOrDefault(fk => fk.DeclaringEntityType.ClrType == typeof(Invoice));

        Assert.NotNull(invoicesForeignKey);
        Assert.Equal(DeleteBehavior.Cascade, invoicesForeignKey.DeleteBehavior);
    }
}
