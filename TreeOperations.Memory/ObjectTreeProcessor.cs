﻿using Staticsoft.TreeOperations.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace Staticsoft.TreeOperations.Memory;

public class ObjectTreeProcessor<Result>
{
    readonly IEnumerable<Operation<Result>> Operations;

    public ObjectTreeProcessor(IEnumerable<Operation<Result>> operations)
        => Operations = operations;

    public async Task<Result> Process(Dictionary<string, object> tree)
    {
        if (!IsOperation(tree)) throw InvalidTree();

        var processed = await ProcessNode(tree);
        return (Result)processed;
    }

    async Task<object> ProcessNode(object node)
    {
        if (node is object[] array) return await ProcessArray(array);
        if (node is not Dictionary<string, object> unprocessed) return node;

        var dictionary = await ProcessPropertyValues(unprocessed);
        if (!IsOperation(dictionary)) return dictionary;

        var operationName = GetNodeType(dictionary);
        var operationData = GetNodeData(dictionary);
        var operation = Operations.Single(operation => operation.Name == operationName);
        var data = ToDataType(operationData, operation.DataType);
        return await operation.Process(data);
    }

    static object ToDataType(object operationData, Type operationDataType)
        => JsonSerializer.Deserialize(JsonSerializer.Serialize(operationData), operationDataType);

    Task<object[]> ProcessArray(object[] array)
        => Task.WhenAll(array.Select(ProcessNode));

    async Task<Dictionary<string, object>> ProcessPropertyValues(Dictionary<string, object> dictionary)
    {
        var processed = await Task.WhenAll(dictionary.Keys.Select(async key =>
        {
            var value = await ProcessNode(dictionary[key]);
            return new { Key = key, Value = value };
        }));
        return processed.ToDictionary(item => item.Key, item => item.Value);
    }

    static bool IsOperation(Dictionary<string, object> dictionary)
        => dictionary.ContainsKey("type") && dictionary.ContainsKey("data");

    static string GetNodeType(Dictionary<string, object> node)
        => node["type"] as string;

    static object GetNodeData(Dictionary<string, object> node)
        => node["data"];

    static FormatException InvalidTree()
        => new($"Invalid tree structure: both 'type' and 'data' properties must be included in the root object");
}
