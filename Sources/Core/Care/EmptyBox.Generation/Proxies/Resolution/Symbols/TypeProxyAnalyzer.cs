using EmptyBox.Generation.Abstractions.Resolution;
using EmptyBox.Generation.Diagnostics;
using EmptyBox.Generation.Extensions;
using EmptyBox.Generation.Proxies.Resolution.Symbols.Qualification;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Resolution.Symbols;

internal partial class TypeProxyAnalyzer : IAnalyzer<TypeProxyAnalyzer.Context, TypeProxyAnalyzer.Report>
{
    public static TypeProxyAnalyzer Instance { get; } = new();

    private TypeProxyAnalyzer() { }

    public Report Analyze(Context context)
    {
        TypeQualificationAnalyzer.Report? qualificationInfo = TypeQualificationAnalyzer.Instance.Analyze(context);

        if (qualificationInfo != null)
        {
            if (!context.Target.IsPartial)
            {
                context.DiagnosticMessages.Add(new SymbolDiagnosticRecord()
                {
                    Identifier = DiagnosticIdentifier.EBSG0000,
                    SyntaxReferences = context.Target.Symbol.DeclaringSyntaxReferences,
                });

                goto EMPTY_REPORT;
            }

            PreliminaryReport typeReport = new()
            {
                QualificationReport = qualificationInfo,
                Target = context.Target
            };

            var interfaceMethods = from @interface in context.Target.Symbol.AllInterfaces
                                   from method in @interface.GetMembers().OfType<IMethodSymbol>()
                                   select method;

            var allMethods = context.Target.Symbol.GetMembers().OfType<IMethodSymbol>().Concat(interfaceMethods);

            var methodAnalyzerReports = from method in allMethods
                                        from implemented in method.EnumerateAllImplementedMethods().DefaultIfEmpty(method)
                                        group method by implemented into implementers
                                        select MethodProxyAnalyzer.Instance.Analyze(new MethodProxyAnalyzer.Context()
                                        {
                                            DiagnosticMessages = context.DiagnosticMessages,
                                            Symbols = context.Symbols,
                                            Target = new MethodProxyAnalyzer.Target()
                                            {
                                                // Предполагается, что первым окажется самое близкое определение метода к анализируемому типу по иерархиям наследования и реализации
                                                Origin = implementers.First(),
                                                Symbol = implementers.Key,
                                                Implementers = [.. implementers]
                                            },
                                            TypeReport = typeReport
                                        }) into report
                                        where report.HasValue
                                        select report.Value;

            ImmutableArray<MethodProxyAnalyzer.Report> enumeratedReports = [.. methodAnalyzerReports];

            if (enumeratedReports.Length > 0)
            {
                return new Report()
                {
                    DiagnosticMessages = [.. context.DiagnosticMessages],
                    IsAsyncExternalProxyRequired = enumeratedReports.Any(static methodReport => methodReport.ServiceReport?.IsAsync ?? false),
                    MethodReports = enumeratedReports,
                    Symbols = context.Symbols,
                    Preliminary = typeReport,
                };
            }
        }

    EMPTY_REPORT:
        return new Report()
        {
            DiagnosticMessages = [.. context.DiagnosticMessages],
            IsAsyncExternalProxyRequired = false,
            MethodReports = [],
            Symbols = context.Symbols,
            Preliminary = new PreliminaryReport()
            {
                QualificationReport = null,
                Target = context.Target
            }
        };
    }
}
