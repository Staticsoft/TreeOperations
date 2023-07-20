using Microsoft.Extensions.DependencyInjection;
using Staticsoft.TreeOperations.Abstractions;
using System;

namespace Staticsoft.TreeOperations.Memory;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection UseJsonTreeProcessor<Result>(
        this IServiceCollection services,
        Func<TreeProcessorBuilder<Result>, TreeProcessorBuilder<Result>> build
    ) => services
        .UseTreeProcessor(build)
        .AddSingleton<TreeProcessor<Result>, JsonTreeProcessor<Result>>();

    static IServiceCollection UseTreeProcessor<Result>(
        this IServiceCollection services,
        Func<TreeProcessorBuilder<Result>, TreeProcessorBuilder<Result>> build
    )
    {
        build(new TreeProcessorBuilder<Result>(services));

        return services
            .AddSingleton<ObjectTreeProcessor<Result>>()
            .AddSingleton<ObjectSerializer>();
    }

    public static IServiceCollection UseOperation<Result, Implementation>(this IServiceCollection services)
        where Implementation : class, Operation<Result>
        => services.AddSingleton<Operation<Result>, Implementation>();
}

public class TreeProcessorBuilder<Result>
{
    readonly IServiceCollection Services;

    public TreeProcessorBuilder(IServiceCollection services)
        => Services = services;

    public TreeProcessorBuilder<Result> With<Operation>()
        where Operation : class, Operation<Result>
    {
        Services.AddSingleton<Operation<Result>, Operation>();
        return this;
    }
}