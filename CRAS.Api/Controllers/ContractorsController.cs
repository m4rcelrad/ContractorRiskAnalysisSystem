using CRAS.Api.Models;
using CRAS.Application.Requests;
using CRAS.Domain.Engine;
using CRAS.Domain.Entities;
using CRAS.Infrastructure.Data;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractorsController(
    AppDbContext context,
    IRiskEngine riskEngine,
    IValidator<AddInvoiceRequest> validator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var contractors = await context.Contractors
            .Include(c => c.FinancialStatements)
            .Include(c => c.Invoices)
            .ToListAsync();

        return Ok(contractors);
    }

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

    [HttpPost("{id:guid}/invoices")]
    public async Task<IActionResult> AddInvoice(Guid id, [FromBody] AddInvoiceRequest request)
    {
        request.ContractorId = id;

        var validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            return BadRequest(validationResult.Errors);
        }

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
}
