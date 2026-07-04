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

    [Fact]
    public async Task GatherDependencyStatus_WithNullRequest_UsesDefaultRequestValues()
    {
        ExternalStatusRequest? capturedRequest = null;
        var mockStatusService = new Mock<IExternalStatusService>();
        mockStatusService
            .Setup(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ExternalStatusRequest, CancellationToken>((request, ct) => capturedRequest = request)
            .ReturnsAsync(Array.Empty<ExternalStatusResult>());

        var controller = new ExternalStatusController(mockStatusService.Object);

        var result = await controller.GatherDependencyStatus(null, CancellationToken.None);

        mockStatusService.Verify(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedRequest);
        Assert.Empty(capturedRequest!.Urls);
        Assert.Equal(5000, capturedRequest.TimeoutMs);
        Assert.Equal(5, capturedRequest.MaxConcurrency);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<ExternalStatusResult>>(okResult.Value);
    }

    [Fact]
    public async Task GatherDependencyStatus_WithUrlListAndCustomSettings_ForwardsValuesToService()
    {
        ExternalStatusRequest? capturedRequest = null;
        var mockStatusService = new Mock<IExternalStatusService>();
        mockStatusService
            .Setup(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ExternalStatusRequest, CancellationToken>((request, ct) => capturedRequest = request)
            .ReturnsAsync(Array.Empty<ExternalStatusResult>());

        var controller = new ExternalStatusController(mockStatusService.Object);
        var request = new ExternalStatusRequest
        {
            Urls = new List<string> { "https://example.com" },
            TimeoutMs = 1234,
            MaxConcurrency = 2,
        };

        var result = await controller.GatherDependencyStatus(request, CancellationToken.None);

        mockStatusService.Verify(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedRequest);
        Assert.Collection(capturedRequest!.Urls, url => Assert.Equal("https://example.com", url));
        Assert.Equal(1234, capturedRequest.TimeoutMs);
        Assert.Equal(2, capturedRequest.MaxConcurrency);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<ExternalStatusResult>>(okResult.Value);
    }

    [Fact]
    public async Task GatherDependencyStatus_WithUrlListAndDefaultSettings_UsesUrlListAndDefaults()
    {
        ExternalStatusRequest? capturedRequest = null;
        var mockStatusService = new Mock<IExternalStatusService>();
        mockStatusService
            .Setup(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ExternalStatusRequest, CancellationToken>((request, ct) => capturedRequest = request)
            .ReturnsAsync(Array.Empty<ExternalStatusResult>());

        var controller = new ExternalStatusController(mockStatusService.Object);
        var request = new ExternalStatusRequest
        {
            Urls = new List<string> { "https://example.com" },
        };

        var result = await controller.GatherDependencyStatus(request, CancellationToken.None);

        mockStatusService.Verify(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedRequest);
        Assert.Collection(capturedRequest!.Urls, url => Assert.Equal("https://example.com", url));
        Assert.Equal(5000, capturedRequest.TimeoutMs);
        Assert.Equal(5, capturedRequest.MaxConcurrency);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<ExternalStatusResult>>(okResult.Value);
    }

    [Fact]
    public async Task GatherDependencyStatus_WithCustomTimeoutOnly_UsesSpecifiedTimeout()
    {
        ExternalStatusRequest? capturedRequest = null;
        var mockStatusService = new Mock<IExternalStatusService>();
        mockStatusService
            .Setup(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ExternalStatusRequest, CancellationToken>((request, ct) => capturedRequest = request)
            .ReturnsAsync(Array.Empty<ExternalStatusResult>());

        var controller = new ExternalStatusController(mockStatusService.Object);
        var request = new ExternalStatusRequest
        {
            Urls = new List<string> { "https://example.com" },
            TimeoutMs = 2000,
        };

        var result = await controller.GatherDependencyStatus(request, CancellationToken.None);

        mockStatusService.Verify(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedRequest);
        Assert.Equal(2000, capturedRequest!.TimeoutMs);
        Assert.Equal(5, capturedRequest.MaxConcurrency);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<ExternalStatusResult>>(okResult.Value);
    }

    [Fact]
    public async Task GatherDependencyStatus_WithCustomMaxConcurrencyOnly_UsesSpecifiedConcurrency()
    {
        ExternalStatusRequest? capturedRequest = null;
        var mockStatusService = new Mock<IExternalStatusService>();
        mockStatusService
            .Setup(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()))
            .Callback<ExternalStatusRequest, CancellationToken>((request, ct) => capturedRequest = request)
            .ReturnsAsync(Array.Empty<ExternalStatusResult>());

        var controller = new ExternalStatusController(mockStatusService.Object);
        var request = new ExternalStatusRequest
        {
            Urls = new List<string> { "https://example.com" },
            MaxConcurrency = 7,
        };

        var result = await controller.GatherDependencyStatus(request, CancellationToken.None);

        mockStatusService.Verify(service => service.CheckStatusAsync(It.IsAny<ExternalStatusRequest>(), It.IsAny<CancellationToken>()), Times.Once);
        Assert.NotNull(capturedRequest);
        Assert.Equal(7, capturedRequest!.MaxConcurrency);
        Assert.Equal(5000, capturedRequest.TimeoutMs);

        var okResult = Assert.IsType<OkObjectResult>(result.Result);
        Assert.IsAssignableFrom<IEnumerable<ExternalStatusResult>>(okResult.Value);
    }
}
