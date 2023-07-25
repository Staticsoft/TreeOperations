using Staticsoft.TreeOperations.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Tests;

public class SumOperation : Operation<int[], int>
{
    protected override Task<int> Process(int[] numbers)
        => Task.FromResult(numbers.Sum());
}
