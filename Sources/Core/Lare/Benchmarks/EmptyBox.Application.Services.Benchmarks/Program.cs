using BenchmarkDotNet.Running;

using EmptyBox.Benchmarks.Shared;

namespace EmptyBox.Application.Services.Benchmarks;

internal class Program
{
    public static void Main(string[] args)
    {
        var summary = BenchmarkRunner.Run(typeof(Program).Assembly, new BaseConfiguration(), args);
    }
}
