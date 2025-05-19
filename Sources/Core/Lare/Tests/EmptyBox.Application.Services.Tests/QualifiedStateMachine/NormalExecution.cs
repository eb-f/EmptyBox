using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace EmptyBox.Application.Services.Tests.QualifiedStateMachine;

[TestClass]
[RequiresDynamicCode("Конструирование машины состояний.")]
public sealed class NormalExecution
{
    [TestMethod]
    public async Task TeapotLifeCycle()
    {
        Teapot<SC> Teapot = QualifiedStateMachineFactory.Instance
                                                        .Initialize<Teapot<SI>>()
                                                        .Configure(new ITeapot.Configuration() { HeatingRate = 10, BaseTemperature = 100 });

        var launched = await Teapot.Launch();
        var heated = await launched.Heat();
        _ = await heated.Stop();
    }
}
