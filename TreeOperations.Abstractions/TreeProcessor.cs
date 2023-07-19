using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Abstractions;

public interface TreeProcessor<Result>
{
    Task<Result> Process(string serializedTree);
}
