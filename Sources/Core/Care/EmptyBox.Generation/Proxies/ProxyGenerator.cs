using EmptyBox.Generation.Diagnostics;
using EmptyBox.Generation.Extensions;
using EmptyBox.Generation.Polyfills;
using EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;
using EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.External;
using EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp.Internal;
using EmptyBox.Generation.Proxies.Resolution.Symbols;
using EmptyBox.Generation.Proxies.Resolution.Syntaxes.CSharp;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies;

[Generator]
public partial class ProxyGenerator : IIncrementalGenerator
{
    private static readonly FileProxyBuilder InternalBuilder = new() { TypeBuilder = TypeInternalProxyBuilder.Instance };
    private static readonly FileProxyBuilder ExternalBuilder = new() { TypeBuilder = TypeExternalProxyBuilder.Instance };

    private static void GenerateDiagnostics(SourceProductionContext productionContext, ImmutableArray<SymbolDiagnosticRecord> records)
    {
        foreach (SymbolDiagnosticRecord record in records)
        {
            var locationsQuery = from syntaxReference in record.SyntaxReferences
                                 select Location.Create(syntaxReference.SyntaxTree, syntaxReference.Span);

            Queue<Location> locations = new(locationsQuery);

            if (!locations.TryDequeue(out Location? firstLocation))
            {
                firstLocation = null;
            }

            productionContext.ReportDiagnostic(Diagnostic.Create(DescriptorsCache.Descriptors[record.Identifier], firstLocation, locations, record.Arguments.IsDefaultOrEmpty ? [] : record.Arguments.ToArray()));
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var syntaxProvider = context.SyntaxProvider.CreateSyntaxProvider(static (node, _) => node is TypeDeclarationSyntax typeDeclaration && typeDeclaration.BaseList?.Types.Count > 0,
                                                                         static (context, token) => new
                                                                         {
                                                                             Declaration = (TypeDeclarationSyntax)context.Node,
                                                                             context.SemanticModel,
                                                                             Symbol = (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!
                                                                         });

        var analyzingPipeline = from syntaxContext in syntaxProvider
                                from compilation in context.CompilationProvider
                                select TypeProxyAnalyzer.Instance.Analyze(new TypeProxyAnalyzer.Context()
                                {
                                    SemanticModel = syntaxContext.SemanticModel,
                                    Symbols = new SymbolCache(compilation),
                                    DiagnosticMessages = [],
                                    Target = new TypeProxyAnalyzer.Target()
                                    {
                                        IsPartial = syntaxContext.Declaration.Modifiers.Any(x => x.Text == "partial"),
                                        Symbol = syntaxContext.Symbol
                                    }
                                });

        var diagnosingPipeline = from report in analyzingPipeline
                                 where report.DiagnosticMessages.Length > 0
                                 select report.DiagnosticMessages;

        var buildingPipeline = from reportHandle in analyzingPipeline
                               where reportHandle.Value.MethodReports.Length > 0
                               select new FileProxyBuilder.Context()
                               {
                                   Type = new TypeProxyBuilder.Context()
                                   {
                                       IsAsyncExternalProxyRequired = reportHandle.Value.IsAsyncExternalProxyRequired,
                                       MethodReports = reportHandle.Value.MethodReports,
                                       Report = reportHandle.Value.Preliminary,
                                       Symbols = reportHandle.Value.Symbols
                                   },
                                   IncludedNamespaces = IncludedNamespaceAnalyzer.Instance.Analyze(new IncludedNamespaceAnalyzer.Context()
                                   {
                                       CancellationToken = reportHandle.CancellationToken,
                                       Compilation = reportHandle.Value.Symbols.Compilation,
                                       Target = reportHandle.Value.Preliminary.Target.Symbol
                                   }).IncludedNamespaces
                               };

        context.RegisterSourceOutput(diagnosingPipeline, GenerateDiagnostics);
        context.RegisterImplementationSourceOutput(buildingPipeline, InternalBuilder.Build);
        context.RegisterSourceOutput(buildingPipeline, ExternalBuilder.Build);
    }
}
