using CRAS.Api.Models;
using CRAS.Domain.Engine;
using CRAS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CRAS.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ContractorsController(AppDbContext context, IRiskEngine riskEngine) : ControllerBase
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
            .FirstOrDefaultAsync(c => c.Id == id);

        if (contractor == null)
        {
            return NotFound("Contractor not found.");
        }

        var latestStatement = contractor.FinancialStatements
            .OrderByDescending(s => s.Year)
            .FirstOrDefault();

        if (latestStatement == null)
        {
            return BadRequest("No financial statements available for this contractor.");
        }

        var result = riskEngine.Assess(latestStatement);

        var response = new RiskAssessmentResponse
        {
            ContractorId = contractor.Id,
            ContractorName = $"Contractor {contractor.TaxId}",
            OverallRisk = result.OverallRiskLevel,
            Breakdown = result.IndividualResults.ToList()
        };

        return Ok(response);
    }
}
