using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Backend.Controllers;
using Backend.Models;
using Backend.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Backend.Tests;

public class ApiEndpointBadPathTests
{
    [Fact]
    public async Task GatherDependencyStatus_WhenStatusServiceThrows_PropagatesException()
    {
        var mockStatusService = new Mock<IExternalStatusService>();
        mockStatusService
            .Setup(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("upstream failure"));

        var controller = new ExternalStatusController(mockStatusService.Object);

        var exception = await Record.ExceptionAsync(() => controller.GatherDependencyStatus(
            new ExternalStatusRequest { TimeoutMs = 100, MaxConcurrency = 1, Urls = new List<string> { "https://example.com" } },
            CancellationToken.None));

        Assert.NotNull(exception);
        Assert.IsType<InvalidOperationException>(exception);
        Assert.Equal("upstream failure", exception.Message);
    }

    [Fact]
    public async Task GetByUrl_WithEmptyUrl_ReturnsBadRequest()
    {
        var options = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<Backend.Data.AppDbContext>()
            .UseSqlite("DataSource=:memory:")
            .Options;

        await using var dbContext = new Backend.Data.AppDbContext(options);
        await dbContext.Database.OpenConnectionAsync();
        await dbContext.Database.EnsureCreatedAsync();

        var historyService = new ExternalStatusHistoryService(dbContext);
        var controller = new ExternalStatusHistoryController(historyService);

        var result = await controller.GetByUrl(string.Empty, 10);

        Assert.IsType<BadRequestObjectResult>(result.Result);
    }
}
