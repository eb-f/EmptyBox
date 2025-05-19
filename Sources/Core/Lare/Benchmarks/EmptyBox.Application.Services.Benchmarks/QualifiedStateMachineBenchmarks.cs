using BenchmarkDotNet.Attributes;

using EmptyBox.Application.Services.Benchmarks.Teapots;
using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;

using System.Threading.Tasks;

namespace EmptyBox.Application.Services.Benchmarks;

public class QualifiedStateMachineBenchmarks
{
    private static readonly ITeapot.Configuration Configuration = new() { HeatingRate = 10, BaseTemperature = 100 };

    private readonly Teapot<SC> Teapot = QualifiedStateMachineFactory.Instance
                                                                     .Initialize<Teapot<SI>>()
                                                                     .Configure(Configuration);
    private readonly SealedTeapot<SC> SealedTeapot = QualifiedStateMachineFactory.Instance
                                                                                 .Initialize<SealedTeapot<SI>>()
                                                                                 .Configure(Configuration);
    private readonly SyncTeapot<SC> SyncTeapot = QualifiedStateMachineFactory.Instance
                                                                             .Initialize<SyncTeapot<SI>>()
                                                                             .Configure(Configuration);
    private readonly SealedSyncTeapot<SC> SealedSyncTeapot = QualifiedStateMachineFactory.Instance
                                                                                         .Initialize<SealedSyncTeapot<SI>>()
                                                                                         .Configure(Configuration);
    private readonly NonQualifiedTeapot NonQualifiedTeapot = new(Configuration);
    private readonly NonQualifiedVirtualTeapot NonQualifiedVirtualTeapot = new(Configuration);
    private readonly NonQualifiedSyncTeapot NonQualifiedSyncTeapot = new(Configuration);
    private readonly NonQualifiedVirtualSyncTeapot NonQualifiedVirtualSyncTeapot = new(Configuration);


    [Benchmark]
    public async Task TeapotLifeCycle()
    {
        var launched = await Teapot.Launch();
        var heated = await launched.Heat();
        _ = await heated.Stop();
    }

    [Benchmark]
    public void SyncTeapotLifeCycle()
    {
        var launched = SyncTeapot.Launch();
        var heated = launched.Heat();
        _ = heated.Stop();
    }

    [Benchmark]
    public async Task SealedTeapotLifeCycle()
    {
        var launched = await SealedTeapot.Launch();
        var heated = await launched.Heat();
        _ = await heated.Stop();
    }

    [Benchmark]
    public void SealedSyncTeapotLifeCycle()
    {
        var launched = SealedSyncTeapot.Launch();
        var heated = launched.Heat();
        _ = heated.Stop();
    }

    [Benchmark]
    public async Task NonQualifiedTeapotLifeCycle()
    {
        await NonQualifiedTeapot.Launch();
        await NonQualifiedTeapot.Heat();
        await NonQualifiedTeapot.Stop();
    }

    [Benchmark]
    public void NonQualifiedSyncTeapotLifeCycle()
    {
        NonQualifiedSyncTeapot.Launch();
        NonQualifiedSyncTeapot.Heat();
        NonQualifiedSyncTeapot.Stop();
    }

    [Benchmark]
    public async Task NonQualifiedVirtualTeapotLifeCycle()
    {
        await NonQualifiedVirtualTeapot.Launch();
        await NonQualifiedVirtualTeapot.Heat();
        await NonQualifiedVirtualTeapot.Stop();
    }

    [Benchmark]
    public void NonQualifiedVirtualSyncTeapotLifeCycle()
    {
        NonQualifiedVirtualSyncTeapot.Launch();
        NonQualifiedVirtualSyncTeapot.Heat();
        NonQualifiedVirtualSyncTeapot.Stop();
    }
}
