using CRAS.Domain.Entities;
using CRAS.Domain.ValueObjects;

namespace CRAS.Domain.Interfaces;

/// <summary>
///     Defines a contract for generating reports based on contractor data and aggregated risk assessment results.
/// </summary>
public interface IReportGenerator
{
    byte[] Generate(Contractor contractor, AggregatedRiskResult assessment);
}
