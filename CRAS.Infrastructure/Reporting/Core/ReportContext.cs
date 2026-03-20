using CRAS.Domain.Entities;
using CRAS.Domain.ValueObjects;
using CRAS.Infrastructure.Reporting.Helpers;

namespace CRAS.Infrastructure.Reporting.Core;

public class ReportContext
{
    public required Contractor Contractor { get; init; }
    public required AggregatedRiskResult Assessment { get; init; }
    public required IStyleProvider Style { get; init; }
}
