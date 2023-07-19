using Staticsoft.TreeOperations.Abstractions;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Tests;

public class SquareOperation : Operation<int, int>
{
    protected override Task<int> Process(int number)
        => Task.FromResult(number * number);
}
