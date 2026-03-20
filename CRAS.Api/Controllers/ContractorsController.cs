using CRAS.Application.Models;
using CRAS.Application.Requests;
using CRAS.Domain.Entities;
using CRAS.Domain.Interfaces;
using CRAS.Domain.Services;
using CRAS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Api.Controllers;

/// <summary>
///     Provides endpoints for managing contractors, invoices, financial statements and performing risk assessments.
/// </summary>
/// <param name="context"></param>
/// <param name="riskEngine"></param>
[ApiController]
[Route("api/[controller]")]
public class ContractorsController(
    AppDbContext context,
    IRiskEngine riskEngine) : ControllerBase
{
    /// <summary>
    ///     Retrieves a list of all contractors, including their financial statements and invoices.
    /// </summary>
    /// <returns>A list of contractors with their full historical data.</returns>
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contractors = await context.Contractors
            .Include(c => c.FinancialStatements)
            .Include(c => c.Invoices)
            .ToListAsync();

        return Ok(contractors);
    }

    /// <summary>
    ///     Performs a risk assessment for a specific contractor based on their latest financial statement and payment history.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor to assess.</param>
    /// <returns>A detailed risk assessment response including overall risk level and breakdown by individual models.</returns>
    /// <response code="200">Returns the calculated risk assessment.</response>
    /// <response code="400">If the contractor has no financial statements to analyze.</response>
    /// <response code="404">If the contractor with the specified ID is not found.</response>
    [HttpGet("{id:guid}/assess")]
    public async Task<IActionResult> AssessRisk(Guid id)
    {
        var contractor = await context.Contractors
            .Include(c => c.FinancialStatements)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contractor == null) return NotFound();

        var latestStatement = contractor.FinancialStatements
            .OrderByDescending(s => s.Year)
            .FirstOrDefault();

        if (latestStatement == null) return BadRequest();

        var result = await riskEngine.AssessAsync(contractor, latestStatement);

        var response = new RiskAssessmentResponse
        {
            ContractorId = contractor.Id,
            ContractorName = $"Contractor {contractor.TaxId}",
            OverallRisk = result.OverallRiskLevel,
            Breakdown = result.IndividualResults.ToList()
        };

        return Ok(response);
    }

    /// <summary>
    ///     Registers a new contractor in the system.
    /// </summary>
    /// <param name="request">The request body containing the Tax ID (NIP).</param>
    /// <returns>The newly created contractor object.</returns>
    /// <response code="201">The contractor was successfully created.</response>
    /// <response code="400">If the validation for Tax ID format or checksum fails.</response>
    [HttpPost]
    public async Task<IActionResult> AddContractor([FromBody] AddContractorRequest request)
    {
        var contractor = new Contractor
        {
            TaxId = request.TaxId
        };

        context.Contractors.Add(contractor);
        await context.SaveChangesAsync();

        return Created($"/api/contractors/{contractor.Id}", contractor);
    }

    /// <summary>
    ///     Adds a new invoice to an existing contractor's record.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor.</param>
    /// <param name="request">Invoice details including amount, currency, and dates.</param>
    /// <returns>The created invoice record.</returns>
    /// <response code="201">The invoice was successfully added.</response>
    /// <response code="404">If the specified contractor does not exist.</response>
    [HttpPost("{id:guid}/invoices")]
    public async Task<IActionResult> AddInvoice(Guid id, [FromBody] AddInvoiceRequest request)
    {
        request.ContractorId = id;

        var contractorExists = await context.Contractors.AnyAsync(c => c.Id == id);

        if (!contractorExists)
        {
            return NotFound();
        }

        var invoice = new Invoice
        {
            ContractorId = id,
            Amount = request.Amount,
            Currency = request.Currency,
            IssueDate = request.IssueDate.ToUniversalTime(),
            DueDate = request.DueDate.ToUniversalTime(),
            IsPaid = false
        };

        context.Invoices.Add(invoice);
        await context.SaveChangesAsync();

        return Created($"/api/contractors/{id}/invoices/{invoice.Id}", invoice);
    }

    /// <summary>
    ///     Adds a new annual financial statement for a specific contractor.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor.</param>
    /// <param name="request">Detailed financial data required for bankruptcy prediction models.</param>
    /// <returns>The created financial statement record.</returns>
    /// <response code="201">The statement was successfully recorded.</response>
    /// <response code="404">If the specified contractor does not exist.</response>
    [HttpPost("{id:guid}/statements")]
    public async Task<IActionResult> AddFinancialStatement(Guid id, [FromBody] AddFinancialStatementRequest request)
    {
        request.ContractorId = id;

        var contractorExists = await context.Contractors.AnyAsync(c => c.Id == id);

        if (!contractorExists)
        {
            return NotFound();
        }

        var statement = new FinancialStatement
        {
            ContractorId = id,
            Year = request.FiscalYear,
            TotalAssets = request.TotalAssets,
            TotalLiabilities = request.TotalLiabilities,
            CurrentAssets = request.CurrentAssets,
            CurrentLiabilities = request.CurrentLiabilities,
            WorkingCapital = request.WorkingCapital,
            RetainedEarnings = request.RetainedEarnings,
            EBIT = request.EBIT,
            MarketValueEquity = request.MarketValueEquity,
            BookValueEquity = request.BookValueEquity,
            Sales = request.Sales,
            NetIncome = request.NetIncome,
            PreviousNetIncome = request.PreviousNetIncome,
            FundsFromOperations = request.FundsFromOperations,
            GNPPriceIndex = request.GNPPriceIndex
        };

        context.FinancialStatements.Add(statement);
        await context.SaveChangesAsync();

        return Created($"/api/contractors/{id}/statements/{statement.Id}", statement);
    }

    /// <summary>
    ///     Retrieves an overview of all contractors, including their risk assessments
    ///     and latest financial statements.
    /// </summary>
    /// <returns>A list containing dashboard overviews for all contractors.</returns>
    [HttpGet("dashboard")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetDashboard()
    {
        var contractors = await context.Contractors
            .Include(c => c.FinancialStatements)
            .Include(c => c.Invoices)
            .ToListAsync();

        var overviews = new List<DashboardOverviewResponse>();

        foreach (var contractor in contractors)
        {
            var latestStatement = contractor.FinancialStatements
                .OrderByDescending(s => s.Year)
                .FirstOrDefault();

            if (latestStatement == null)
            {
                continue;
            }

            var result = await riskEngine.AssessAsync(contractor, latestStatement);

            overviews.Add(new DashboardOverviewResponse
            {
                ContractorId = contractor.Id,
                TaxId = contractor.TaxId,
                Assessment = new RiskAssessmentResponse
                {
                    ContractorId = contractor.Id,
                    ContractorName = $"Contractor {contractor.TaxId}",
                    OverallRisk = result.OverallRiskLevel,
                    Breakdown = result.IndividualResults.ToList()
                }
            });
        }

        return Ok(overviews);
    }

    /// <summary>
    ///     Retrieves details of a specific contractor by their unique identifier, including associated financial statements
    ///     and invoices.
    /// </summary>
    /// <param name="id">The unique identifier of the contractor.</param>
    /// <returns>
    ///     An HTTP response containing the contractor's details if found, or a "Not Found" status if the contractor does
    ///     not exist.
    /// </returns>
    [HttpGet("{id:guid}")]
    [OutputCache(Duration = 60)]
    public async Task<IActionResult> GetContractor(Guid id)
    {
        var contractor = await context.Contractors
            .Include(c => c.FinancialStatements)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contractor == null) return NotFound();

        return Ok(contractor);
    }

    /// <summary>
    ///     Downloads a detailed PDF report for a specific contractor, including financial evaluations and risk assessments.
    /// </summary>
    /// <param name="id">The unique identifier (GUID) of the contractor whose report is to be generated.</param>
    /// <param name="generator">The report generator service responsible for creating the PDF file.</param>
    /// <returns>
    ///     A PDF file containing the contractor's report or an appropriate HTTP response if the contractor or data is
    ///     missing.
    /// </returns>
    [HttpGet("{id:guid}/report")]
    [OutputCache(Duration = 300)]
    public async Task<IActionResult> DownloadReport(Guid id, [FromServices] IReportGenerator generator)
    {
        var contractor = await context.Contractors
            .Include(c => c.FinancialStatements)
            .Include(c => c.Invoices)
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contractor == null) return NotFound();

        var latestStatement = contractor.FinancialStatements
            .OrderByDescending(s => s.Year)
            .FirstOrDefault();

        if (latestStatement == null) return BadRequest();

        var result = await riskEngine.AssessAsync(contractor, latestStatement);

        var pdfBytes = generator.Generate(contractor, result);

        return File(pdfBytes, "application/pdf", $"Report_{contractor.TaxId}.pdf");
    }
}
