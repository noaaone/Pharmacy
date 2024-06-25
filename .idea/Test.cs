using System;
using Xunit;

public class BusinessProcessServiceTests
{
    [Fact]
    public void AddProcess_ShouldAddProcess()
    {
        var service = new BusinessProcessService();
        var process = new BusinessProcess("Order Processing", TimeSpan.FromMinutes(30));

        service.AddProcess(process);

        Assert.Contains(process, service.GetAllProcesses());
    }

    [Fact]
    public void OptimizeProcess_ShouldReduceDuration()
    {
        var service = new BusinessProcessService();
        var process = new BusinessProcess("Order Processing", TimeSpan.FromMinutes(30));
        service.AddProcess(process);

        var optimizedProcess = service.OptimizeProcess("Order Processing");

        Assert.NotNull(optimizedProcess);
        Assert.True(optimizedProcess.IsOptimized);
        Assert.Equal(TimeSpan.FromMinutes(25), optimizedProcess.Duration);
    }

    [Fact]
    public void GetTotalDuration_ShouldReturnSumOfDurations()
    {
        var service = new BusinessProcessService();
        service.AddProcess(new BusinessProcess("Order Processing", TimeSpan.FromMinutes(30)));
        service.AddProcess(new BusinessProcess("Shipping", TimeSpan.FromMinutes(45)));

        var totalDuration = service.GetTotalDuration();

        Assert.Equal(TimeSpan.FromMinutes(75), totalDuration);
    }

    [Fact]
    public void OptimizeProcess_ShouldNotOptimizeAlreadyOptimizedProcess()
    {
        var service = new BusinessProcessService();
        var process = new BusinessProcess("Order Processing", TimeSpan.FromMinutes(30));
        service.AddProcess(process);

        service.OptimizeProcess("Order Processing");
        var optimizedProcess = service.OptimizeProcess("Order Processing");

        Assert.Equal(TimeSpan.FromMinutes(25), optimizedProcess.Duration);
    }
}