using System.Reflection;
using CRAS.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Infrastructure.Data;

/// <summary>
///     Represents the application's database context, providing access to the database
///     and enabling interaction with the entities defined in the application such as
///     <see cref="Contractor" />, <see cref="FinancialStatement" />, and <see cref="Invoice" />.
/// </summary>
/// <remarks>
///     This class inherits from <see cref="Microsoft.EntityFrameworkCore.DbContext" /> and uses
///     Entity Framework Core to define and manage the mapping between entity classes and database tables.
/// </remarks>
public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<Contractor> Contractors => Set<Contractor>();
    public DbSet<FinancialStatement> FinancialStatements => Set<FinancialStatement>();
    public DbSet<Invoice> Invoices => Set<Invoice>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}