using EmptyBox.Generation.Abstractions.Resolution;
using EmptyBox.Generation.Extensions;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

internal partial class MethodQualificationAnalyzer : IAnalyzer<MethodProxyAnalyzer.Context, MethodQualificationAnalyzer.Report?>
{
    public static MethodQualificationAnalyzer Instance { get; } = new();

    private MethodQualificationAnalyzer() { }

    public Report? Analyze(MethodProxyAnalyzer.Context context)
    {
        if (context.TypeReport.QualificationReport != null)
        {
            #region Выпрямление доступа
            INamedTypeSymbol targetType = context.TypeReport.Target.Symbol;
            IMethodSymbol targetMethod = context.Target.Symbol;
            ITypeParameterSymbol targetQualifierTypeParameter = context.TypeReport.QualificationReport.Value.QualifierParameter;
            INamedTypeSymbol qualifiedAttribute = context.Symbols.Qualification.Base.QualifiedAttribute!; // Есть отчёт - значит и символ атрибута в наличии
            #endregion

            var qualifiersQuery = from method in context.Target.Implementers
                                  from attribute in method.GetAttributes()
                                  where attribute.AttributeClass?.IsAssignableTo(qualifiedAttribute) ?? false
                                  group attribute.ApplicationSyntaxReference by Helpers.GetAttributeTypeArgument(method, attribute) into qualifierInfo
                                  select new
                                  {
                                      Qualifier = qualifierInfo.Key,
                                      Applications = qualifierInfo.Where(static reference => reference != null).ToImmutableArray()
                                  };

            List<SingleQualificationReport> qualifications = [];

            foreach (var qualifierInfo in qualifiersQuery)
            {
                if (qualifierInfo.Qualifier == null)
                {
                    context.DiagnosticMessages.Add(new Diagnostics.SymbolDiagnosticRecord()
                    {
                        Identifier = Diagnostics.DiagnosticIdentifier.EBSG0008,
                        SyntaxReferences = qualifierInfo.Applications,
                    });
                    continue;
                }
                
                if (qualifierInfo.Applications.Length > 1)
                {
                    // TODO Диагностическое сообщение о повторяющейся квалификации
                }

                qualifications.Add(new SingleQualificationReport() { Qualification = qualifierInfo.Qualifier });
            }

            if (qualifications.Count > 0)
            {
                return new Report()
                {                  // в случае изначального определения метода
                    IsProxyRequired = SymbolEqualityComparer.Default.Equals(targetMethod.ContainingType, targetType)
                                   || targetType.TypeKind == TypeKind.Structure
                                   || targetType.TypeParameters.Length > 0
                                   // или в случае конкретного аргумента(не параметра)
                                   && targetMethod.ContainingType.TypeArguments.Any(x => x is not ITypeParameterSymbol)
                                   // и на квалификатор не накладывается больше ограничений, чем в типе изначального определения метода
                                   && targetMethod.ContainingType.TypeParameters[0].ConstraintTypes.SequenceEqual(targetType.TypeParameters[0].ConstraintTypes),
                    Qualifications = [.. qualifications]
                };
            }
            else
            {
                return null;
            }
        }
        else
        {
            return null;
        }
    }
}
