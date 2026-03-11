using CRAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CRAS.Infrastructure.Data.Configurations;

/// <summary>
///     Configures the Entity Framework Core database mapping for the <see cref="Contractor" /> entity.
/// </summary>
public class ContractorConfiguration : IEntityTypeConfiguration<Contractor>
{
    /// <summary>
    ///     Configures the properties, database types, and relationships for the contractor.
    /// </summary>
    /// <param name="builder">The builder used to construct the database schema for the entity.</param>
    public void Configure(EntityTypeBuilder<Contractor> builder)
    {
        builder.HasKey(c => c.Id);

        builder.Property(c => c.TaxId).IsRequired().HasMaxLength(10);

        builder.HasMany(c => c.Invoices)
            .WithOne(i => i.Contractor)
            .HasForeignKey(i => i.ContractorId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}