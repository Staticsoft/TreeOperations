using Microsoft.Extensions.DependencyInjection;
using Staticsoft.TreeOperations.Abstractions;
using Staticsoft.TreeOperations.Memory;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using Xunit;

namespace Staticsoft.TreeOperations.Tests;

public class TreeProcessorTests
{
    [Test]
    public async Task ThrowsParseExceptionOnInvalidConfiguration()
    {
        await Assert.ThrowsAsync<FormatException>(() => Process(new { Invalid = "Configuration" }));
    }

    [Test]
    public async Task ProcessesSingleOperationOnSingleElement()
    {
        var result = await Process(new
        {
            Type = "Square",
            Data = 2
        });
        Assert.Equal(4, result);
    }

    [Test]
    public async Task ProcessesSingleOperationOnMultipleElements()
    {
        var result = await Process(new
        {
            Type = "Sum",
            Data = new[] { 1, 2 }
        });
        Assert.Equal(3, result);
    }

    [Test]
    public async Task ProcessesOperationTree()
    {
        var result = await Process(new
        {
            Type = "Sum",
            Data = new[]
            {
                new
                {
                    Type = "Sum",
                    Data = new[] { 1, 2 }
                },
                new
                {
                    Type = "Sum",
                    Data = new[] { 3, 4 }
                }
            }
        });
        Assert.Equal(10, result);
    }

    const int SlightlyMoreThanOneSecond = 1100;

    [Test(Timeout = SlightlyMoreThanOneSecond)]
    public async Task ProcessesOperationsInParallel()
    {
        var result = await Process(new
        {
            Type = "Sum",
            Data = new[]
            {
                new
                {
                    Type = "SleepOneSecondAndSum",
                    Data = new[] { 0 }
                },
                new
                {
                    Type = "SleepOneSecondAndSum",
                    Data = new[] { 0 }
                }
            }
        });
        Assert.Equal(0, result);
    }

    [Test]
    public async Task ProcessesOperationsWithComplexDataTypes()
    {
        var result = await Process(
        new
        {
            Type = "Arithmetic",
            Data = new
            {
                FirstNumber = 3,
                SecondNumber = 2,
                Operation = "Subtract"
            }
        });
        Assert.Equal(1, result);
    }

    static Task<int> Process<Configuration>(Configuration configuration)
    {
        var processor = new ServiceCollection()
            .UseJsonTreeProcessor<int>(tree => tree
                .With<SumOperation>()
                .With<SquareOperation>()
                .With<SleepOneSecondAndSumOperation>()
                .With<ArithmeticOperation>()
            )
            .BuildServiceProvider()
            .GetRequiredService<TreeProcessor<int>>();

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };
        return processor.Process(JsonSerializer.Serialize(configuration, options));
    }
}
