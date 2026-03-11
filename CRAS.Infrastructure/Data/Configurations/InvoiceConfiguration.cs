using CRAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRAS.Infrastructure.Data.Configurations;

/// <summary>
///     Configures the Entity Framework Core database mapping for the <see cref="Invoice" /> entity.
/// </summary>
public class InvoiceConfiguration : IEntityTypeConfiguration<Invoice>
{
    /// <summary>
    ///     Configures the properties, database types, and relationships for the financial statement.
    /// </summary>
    /// <param name="builder">The builder used to construct the database schema for the entity.</param>
    public void Configure(EntityTypeBuilder<Invoice> builder)
    {
        builder.HasKey(i => i.Id);

        builder.Property(i => i.Amount)
            .IsRequired()
            .HasColumnType("numeric(18,2)");

        builder.Property(i => i.Currency)
            .IsRequired()
            .HasMaxLength(3);

        builder.Property(i => i.IssueDate)
            .IsRequired();

        builder.Property(i => i.DueDate)
            .IsRequired();

        builder.Property(i => i.IsPaid)
            .IsRequired();

        builder.Ignore(i => i.DelayInDays);

        builder.HasOne(i => i.Contractor)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => i.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
