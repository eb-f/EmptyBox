using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Diagnosers;
using BenchmarkDotNet.Diagnostics.Windows;
using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Loggers;

using System.Linq;

namespace EmptyBox.Benchmarks.Shared;

public class BaseConfiguration : ManualConfig
{
    public BaseConfiguration()
    {
        DefaultConfig f;
        AddDiagnoser(new MemoryDiagnoser(new MemoryDiagnoserConfig(false)), new JitStatsDiagnoser());
        AddLogger(ConsoleLogger.Default);
        AddExporter(MarkdownExporter.GitHub);
        AddAnalyser(DefaultConfig.Instance.GetAnalysers().ToArray());
        AddValidator(DefaultConfig.Instance.GetValidators().ToArray());
        AddColumnProvider(DefaultConfig.Instance.GetColumnProviders().ToArray());
        AddJob(new SimpleJobAttribute(RuntimeMoniker.Net90).Config.GetJobs().First().WithArguments(new Argument[] {new MsBuildArgument("/p:GeneratePackageOnBuild=false") }));
    }
}
