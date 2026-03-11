using CRAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRAS.Infrastructure.Data.Configurations;

/// <summary>
///     Configures the Entity Framework Core database mapping for the <see cref="FinancialStatement" /> entity.
/// </summary>
public class FinancialStatementConfiguration : IEntityTypeConfiguration<FinancialStatement>
{
    /// <summary>
    ///     Configures the properties, database types, and relationships for the financial statement.
    /// </summary>
    /// <param name="builder">The builder used to construct the database schema for the entity.</param>
    public void Configure(EntityTypeBuilder<FinancialStatement> builder)
    {
        builder.HasKey(f => f.Id);

        builder.Property(f => f.Year)
            .IsRequired();

        builder.Property(f => f.TotalAssets)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.TotalLiabilities)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.CurrentAssets)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.CurrentLiabilities)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.WorkingCapital)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.RetainedEarnings)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.EBIT)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.MarketValueEquity)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.BookValueEquity)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.Sales)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.NetIncome)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.PreviousNetIncome)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.FundsFromOperations)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(f => f.GNPPriceIndex)
            .IsRequired()
            .HasColumnType("numeric(18,4)");

        builder.HasOne(f => f.Contractor)
            .WithMany(c => c.FinancialStatements)
            .HasForeignKey(f => f.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}