using Staticsoft.TreeOperations.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Memory;

public class JsonTreeProcessor<Result> : TreeProcessor<Result>
{
    readonly ObjectTreeProcessor<Result> Processor;

    public JsonTreeProcessor(ObjectTreeProcessor<Result> processor)
        => Processor = processor;

    public Task<Result> Process(string serializedTree)
    {
        var tree = Deserialize(serializedTree);
        return Processor.Process(tree);
    }

    static Dictionary<string, object> Deserialize(string serializedTree)
    {
        var tree = JsonDocument.Parse(serializedTree).RootElement;
        if (tree.ValueKind != JsonValueKind.Object) throw new FormatException();

        return (Deserialize(tree) as Dictionary<string, object>)!;
    }

    static object Deserialize(JsonElement node) => node.ValueKind switch
    {
        JsonValueKind.False => false,
        JsonValueKind.True => true,
        JsonValueKind.Number => ParseNumber(node),
        JsonValueKind.String => node.GetString()!,
        JsonValueKind.Array => node.EnumerateArray().Select(Deserialize).ToArray(),
        JsonValueKind.Object => node.EnumerateObject().ToDictionary(property => property.Name, property => Deserialize(property.Value)),
        _ => throw new FormatException($"Unsupported {nameof(JsonValueKind)} '{node.ValueKind}'")
    };

    static object ParseNumber(JsonElement node)
        => node.TryGetInt32(out var number)
        ? number
        : node.GetDouble();
}
