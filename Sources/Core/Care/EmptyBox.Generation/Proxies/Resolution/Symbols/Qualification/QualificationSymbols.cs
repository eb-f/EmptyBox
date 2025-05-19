using Microsoft.CodeAnalysis;

using System.Diagnostics.CodeAnalysis;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal readonly struct QualificationSymbols(Compilation compilation)
{
    internal readonly struct BaseTarget(Compilation compilation)
    {
        public INamedTypeSymbol? IQualified_1 { get; } = compilation.GetTypeByMetadataName("EmptyBox.Presentation.Permissions.IQualified`1")?
                                                                    .ConstructUnboundGenericType();
        public INamedTypeSymbol? QualifiedAttribute { get; } = compilation.GetTypeByMetadataName("EmptyBox.Presentation.Permissions.QualifiedAttribute");
        public INamedTypeSymbol? InvalidQualificationException { get; } = compilation.GetTypeByMetadataName("EmptyBox.Presentation.Permissions.InvalidQualificationException");
        [MemberNotNullWhen(true, nameof(IQualified_1), nameof(QualifiedAttribute), nameof(InvalidQualificationException))]
        public bool IsAvailable => IQualified_1 != null
                                && QualifiedAttribute != null
                                && InvalidQualificationException != null;
    }

    internal readonly struct ServiceTarget(Compilation compilation)
    {
        public INamedTypeSymbol? IService_1 { get; } = compilation.GetTypeByMetadataName("EmptyBox.Application.Services.IService`1")?
                                                                  .ConstructUnboundGenericType();
        public INamedTypeSymbol? SwitchToAttribute { get; } = compilation.GetTypeByMetadataName("EmptyBox.Application.Services.SwitchToAttribute");
        public INamedTypeSymbol? IStateMachine { get; } = compilation.GetTypeByMetadataName("EmptyBox.Construction.Machines.IStateMachine");
        public INamedTypeSymbol? ContractViolationException { get; } = compilation.GetTypeByMetadataName("EmptyBox.Application.Services.ContractViolationException");
        [MemberNotNullWhen(true, nameof(IService_1), nameof(SwitchToAttribute), nameof(ContractViolationException))]
        public bool IsAvailable => IService_1 != null
                                && SwitchToAttribute != null
                                && ContractViolationException != null;
    }

    public BaseTarget Base { get; } = new(compilation);
    public ServiceTarget Service { get; } = new(compilation);
}