using Staticsoft.TreeOperations.Abstractions;
using System.Linq;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Tests;

public class SleepOneSecondAndSumOperation : Operation<int, int[]>
{
    protected override async Task<int> Process(int[] numbers)
    {
        await Task.Delay(1000);
        return numbers.Sum();
    }
}
