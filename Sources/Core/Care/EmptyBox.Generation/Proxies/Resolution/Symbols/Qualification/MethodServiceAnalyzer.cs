using EmptyBox.Generation.Abstractions.Resolution;
using EmptyBox.Generation.Diagnostics;
using EmptyBox.Generation.Extensions;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class MethodServiceAnalyzer : IAnalyzer<MethodProxyAnalyzer.Context, MethodServiceAnalyzer.Report?>
{
    private static bool IsImplementsServiceInterface(MethodProxyAnalyzer.Context context)
    {
        INamedTypeSymbol serviceInterface = context.Symbols.Qualification.Service.IService_1;
        INamedTypeSymbol targetType = context.TypeReport.Target.Symbol;

        if (!targetType.AllInterfaces.Any(@interface => @interface.IsGenericType && SymbolEqualityComparer.Default.Equals(@interface.ConstructUnboundGenericType(), serviceInterface)))
        {
            context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
            {
                Arguments = [targetType.Name, serviceInterface.Name],
                Identifier = DiagnosticIdentifier.EBSG0003,
                SyntaxReferences = context.TypeReport.Target.Symbol.DeclaringSyntaxReferences,
            });

            return false;
        }
        else
        {
            return true;
        }
    }

    public static MethodServiceAnalyzer Instance { get; } = new();

    private MethodServiceAnalyzer() { }

    public Report? Analyze(MethodProxyAnalyzer.Context context)
    {
        if (context.TypeReport.QualificationReport != null && context.Symbols.Qualification.Service.IsAvailable)
        {
            #region Выпрямление доступа
            INamedTypeSymbol switchToAttribute = context.Symbols.Qualification.Service.SwitchToAttribute;
            INamedTypeSymbol targetType = context.TypeReport.Target.Symbol;
            IMethodSymbol targetMethod = context.Target.Symbol;
            #endregion

            var switchToQualifiersQuery = from method in context.Target.Implementers
                                          from attribute in method.GetReturnTypeAttributes()
                                          where attribute.AttributeClass?.IsAssignableTo(switchToAttribute) ?? false
                                          group attribute.ApplicationSyntaxReference by Helpers.GetAttributeTypeArgument(method, attribute) into qualifierInfo
                                          select new
                                          {
                                              Qualifier = qualifierInfo.Key,
                                              Applications = qualifierInfo.Where(static reference => reference != null).ToImmutableArray()
                                          };

            ITypeSymbol? switchToQualifier = switchToQualifiersQuery.FirstOrDefault()?.Qualifier;

            if (switchToQualifier != null)
            {
                bool isCorrect = true;

                isCorrect &= IsImplementsServiceInterface(context);

                INamedTypeSymbol returnType = targetType;
                bool isAsync = false;

                if (SymbolEqualityComparer.Default.Equals(targetMethod.ReturnType, context.Symbols.Base.ValueTask[0]))
                {
                    isAsync = true;
                    returnType = context.Symbols.Base.ValueTask[1]!.Construct(targetType);
                }
                else if (SymbolEqualityComparer.Default.Equals(targetMethod.ReturnType, context.Symbols.Base.Task[0]))
                {
                    isAsync = true;
                    returnType = context.Symbols.Base.Task[1]!.Construct(targetType);
                }
                else if (!targetMethod.ReturnsVoid)
                {
                    isCorrect = false;
                    context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
                    {
                        Arguments = [targetMethod.Name, switchToAttribute.Name],
                        Identifier = DiagnosticIdentifier.EBSG0006,
                        SyntaxReferences = context.Target.Symbol.DeclaringSyntaxReferences,
                    });
                }

                if (isCorrect)
                {
                    return new Report()
                    {
                        IsAsync = isAsync,
                        OverriddenReturnType = returnType,
                        SwitchToQualification = switchToQualifier,
                    };
                }
            }
        }

        return null;
    }
}
