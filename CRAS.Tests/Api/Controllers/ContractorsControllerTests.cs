using CRAS.Api.Controllers;
using CRAS.Application.Requests;
using CRAS.Domain.Entities;
using CRAS.Domain.Services;
using CRAS.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace CRAS.Tests.Api.Controllers;

/// <summary>
///     Unit tests for the <see cref="ContractorsController" /> class, verifying its functionality and ensuring correct
///     behavior
///     under various conditions.
/// </summary>
/// <remarks>
///     This test class leverages an in-memory database context for isolating and simulating database operations.
///     Mocking is used to replace external dependencies such as the <see cref="IRiskEngine" /> service.
/// </remarks>
public class ContractorsControllerTests
{
    /// <summary>
    ///     Creates and returns a new instance of the application database context configured
    ///     with a unique in-memory database provider.
    /// </summary>
    /// <returns>A fresh instance of <see cref="AppDbContext" />.</returns>
    private static AppDbContext CreateInMemoryDbContext()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        return new AppDbContext(options);
    }

    /// <summary>
    ///     Verifies that the <see cref="ContractorsController.GetContractor" /> method returns
    ///     a <see cref="NotFoundResult" /> when the specified contractor ID does not exist in the database.
    /// </summary>
    [Fact]
    public async Task GetContractor_WhenContractorDoesNotExist_ReturnsNotFound()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var mockRiskEngine = new Mock<IRiskEngine>();
        var controller = new ContractorsController(dbContext, mockRiskEngine.Object);

        var result = await controller.GetContractor(Guid.NewGuid());

        Assert.IsType<NotFoundResult>(result);
    }

    /// <summary>
    ///     Verifies that the <see cref="ContractorsController.GetContractor" /> method returns
    ///     an <see cref="OkObjectResult" /> containing the correct contractor data when a matching ID is found.
    /// </summary>
    [Fact]
    public async Task GetContractor_WhenContractorExists_ReturnsOkWithData()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var mockRiskEngine = new Mock<IRiskEngine>();
        var controller = new ContractorsController(dbContext, mockRiskEngine.Object);

        var contractorId = Guid.NewGuid();
        dbContext.Contractors.Add(new Contractor { Id = contractorId, TaxId = "7740001454" });
        await dbContext.SaveChangesAsync();

        var result = await controller.GetContractor(contractorId);

        var okResult = Assert.IsType<OkObjectResult>(result);
        var returnedContractor = Assert.IsType<Contractor>(okResult.Value);
        Assert.Equal("7740001454", returnedContractor.TaxId);
    }

    /// <summary>
    ///     Verifies that the <see cref="ContractorsController.AddContractor" /> method successfully
    ///     creates a new contractor record and returns a <see cref="CreatedResult" />.
    /// </summary>
    [Fact]
    public async Task AddContractor_CreatesContractor_AndReturnsCreatedResult()
    {
        await using var dbContext = CreateInMemoryDbContext();
        var mockRiskEngine = new Mock<IRiskEngine>();
        var controller = new ContractorsController(dbContext, mockRiskEngine.Object);

        var request = new AddContractorRequest { TaxId = "7740001454" };

        var result = await controller.AddContractor(request);

        var createdResult = Assert.IsType<CreatedResult>(result);
        var createdContractor = Assert.IsType<Contractor>(createdResult.Value);

        Assert.Equal("7740001454", createdContractor.TaxId);
        Assert.Single(dbContext.Contractors);
    }
}
