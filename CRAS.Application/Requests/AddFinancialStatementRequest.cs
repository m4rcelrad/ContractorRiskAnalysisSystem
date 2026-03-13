namespace CRAS.Application.Requests;

public class AddFinancialStatementRequest
{
    public Guid ContractorId { get; set; }
    public int FiscalYear { get; set; }
    public decimal TotalAssets { get; set; }
    public decimal TotalLiabilities { get; set; }
    public decimal CurrentAssets { get; set; }
    public decimal CurrentLiabilities { get; set; }
    public decimal WorkingCapital { get; set; }
    public decimal RetainedEarnings { get; set; }
    public decimal EBIT { get; set; }
    public decimal MarketValueEquity { get; set; }
    public decimal BookValueEquity { get; set; }
    public decimal Sales { get; set; }
    public decimal NetIncome { get; set; }
    public decimal PreviousNetIncome { get; set; }
    public decimal FundsFromOperations { get; set; }
    public decimal GNPPriceIndex { get; set; }
}
