using System;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Abstractions;

public interface Operation
{
    string Name
        => GetType().Name.Replace(nameof(Operation), string.Empty);
}

public interface Operation<Result> : Operation
{
    Task<Result> Process(object properties);
    Type DataType { get; }
}

public abstract class Operation<Data, Result> : Operation<Result>
{
    public Task<Result> Process(object properties)
    {
        var data = (Data)properties;
        return Process(data);
    }

    public Type DataType
        => typeof(Data);

    protected abstract Task<Result> Process(Data data);
}
