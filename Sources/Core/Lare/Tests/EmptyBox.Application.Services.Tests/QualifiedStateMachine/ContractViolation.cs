using EmptyBox.Application.Services.Operation;
using EmptyBox.Application.Services.Shared;
using EmptyBox.Construction.Machines;
using EmptyBox.Presentation.Permissions;

using Microsoft.VisualStudio.TestTools.UnitTesting;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace EmptyBox.Application.Services.Tests.QualifiedStateMachine;

[TestClass]
[RequiresDynamicCode("Конструирование машины состояний.")]
internal class ContractViolation
{
    [TestMethod]
    public async Task QualifiedViolation()
    {
        Teapot<SC> Teapot = QualifiedStateMachineFactory.Instance
                                                        .Initialize<Teapot<SI>>()
                                                        .Configure(new ITeapot.Configuration() { HeatingRate = 10 });

        var launched = await Teapot.Launch();
        await Assert.ThrowsExceptionAsync<InvalidQualificationException>(async () => await Teapot.Launch());
    }

    [TestMethod]
    public async Task SwitchToByExceptionViolation()
    {
        Teapot<SC> Teapot = QualifiedStateMachineFactory.Instance
                                                        .Initialize<Teapot<SI>>()
                                                        .Configure(new ITeapot.Configuration() { HeatingRate = 0 });

        var launched = await Teapot.Launch();
        await Assert.ThrowsExceptionAsync<ContractViolationException>(async () => await launched.Heat());

        switch (launched)
        {
            case Teapot<ITeapot.RequireMaintenance> broken:
                Assert.Inconclusive();

                break;
        }
        _ = await launched.Stop();
    }

    [TestMethod]
    public async Task SwitchToSpecialViolation()
    {
        Teapot<SC> Teapot = QualifiedStateMachineFactory.Instance
                                                        .Initialize<Teapot<SI>>()
                                                        .Configure(new ITeapot.Configuration() { HeatingRate = double.NaN });

        var launched = await Teapot.Launch();
        await Assert.ThrowsExceptionAsync<ContractViolationException>(async () => await launched.Heat());

        switch (launched)
        {
            case Teapot<ITeapot.RequireMaintenance> broken:
                launched = await broken.Maintenance();

                break;
        }
        _ = await launched.Stop();
    }
}
