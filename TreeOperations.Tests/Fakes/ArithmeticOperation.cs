using Staticsoft.TreeOperations.Abstractions;
using System;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Tests;

public class ArithmeticOperation : Operation<int, ArithmeticOperation.Properties>
{
    protected override Task<int> Process(Properties data)
        => Task.FromResult(ProcessOperation(data));

    static int ProcessOperation(Properties data) => data.Operation switch
    {
        Operation.Add => data.FirstNumber + data.SecondNumber,
        Operation.Subtract => data.FirstNumber - data.SecondNumber,
        _ => throw new NotSupportedException($"Operation '{data.Operation}' is not suppored")
    };

    public class Properties
    {
        public int FirstNumber { get; init; }
        public int SecondNumber { get; init; }
        public Operation Operation { get; init; } = Operation.Add;
    }

    public enum Operation
    {
        Add,
        Subtract
    }
}