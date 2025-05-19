using EmptyBox.Generation.Abstractions.Resolution;
using EmptyBox.Generation.Extensions;
using EmptyBox.Generation.Polyfills;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;

namespace EmptyBox.Generation.Proxies.Resolution.Syntaxes.CSharp;

[SuppressMessage("Style", "IDE0036:Order modifiers", Justification = "Ложноположительное срабатывание диагностики")]
file readonly struct SymbolWithLocations
{
    public required INamedTypeSymbol Symbol { get; init; }
    public required ImmutableArray<SyntaxTree> Locations { get; init; }
}

internal class IncludedNamespaceAnalyzer : IAnalyzer<IncludedNamespaceAnalyzer.Context, IncludedNamespaceAnalyzer.Report>
{
    public readonly struct Context
    {
        public required Compilation Compilation { get; init; }
        public required INamedTypeSymbol Target { get; init; }
        public CancellationToken CancellationToken { get; init; }
    }

    public readonly struct Report
    {
        public required ImmutableArray<INamespaceSymbol> IncludedNamespaces { get; init; }
    }

    public static IncludedNamespaceAnalyzer Instance { get; } = new();

    private IncludedNamespaceAnalyzer() { }

    public Report Analyze(Context context)
    {
        HashSet<INamespaceSymbol> namespaces = [];

        ImmutableArray<SymbolWithLocations> namespaceProviders = Enumerable.Repeat(context.Target, 1)
                                                                           .Concat(context.Target.EnumerateBaseTypes())
                                                                           .Concat(context.Target.AllInterfaces)
                                                                           .Where(symbol => SymbolEqualityComparer.Default.Equals(symbol.ContainingAssembly, context.Compilation.Assembly))
                                                                           .Select(static symbol => new SymbolWithLocations()
                                                                           {
                                                                               Locations = symbol.Locations.Where(static location => location.IsInSource)
                                                                                                           .Select(static location => location.SourceTree!)
                                                                                                           .ToImmutableArray(),
                                                                               Symbol = symbol,
                                                                           })
                                                                           .Where(static provider => provider.Locations.Length > 0)
                                                                           .ToImmutableArray();

        foreach (SymbolWithLocations provider in namespaceProviders)
        {
            foreach (SyntaxTree tree in provider.Locations)
            {
                if (tree.GetRoot() is CompilationUnitSyntax compilationUnit)
                {
                    SemanticModel semanticModel = context.Compilation.GetSemanticModel(tree);
                    Queue<SyntaxNode?> nodes = new(compilationUnit.ChildNodes());
                    Stack<INamespaceSymbol?> includedNamespaces = [];

                    while (nodes.TryDequeue(out SyntaxNode? node))
                    {
                        switch (node)
                        {
                            case TypeDeclarationSyntax typeDeclaration:
                                if (SymbolEqualityComparer.Default.Equals(semanticModel.GetDeclaredSymbol(typeDeclaration, context.CancellationToken), provider.Symbol))
                                {
                                    goto LOOP_DONE;
                                }

                                break;

                            case UsingDirectiveSyntax usingDirective:
                                // Псевдонимы автоматически заменяются на полное имя типа
                                if (usingDirective.Alias != null)
                                {
                                    break;
                                }
                                else if (usingDirective.Name is not QualifiedNameSyntax name)
                                {
                                    break;
                                }
                                else if (semanticModel.GetSymbolInfo(name, context.CancellationToken).Symbol is not INamespaceSymbol @namespace)
                                {
                                    break;
                                }
                                else if (@namespace.ContainingAssembly == null || SymbolEqualityComparer.Default.Equals(@namespace.ContainingAssembly, provider.Symbol.ContainingAssembly))
                                {
                                    includedNamespaces.Push(@namespace);
                                }

                                break;

                            case BaseNamespaceDeclarationSyntax namespaceDeclaration:
                                foreach (SyntaxNode childNode in namespaceDeclaration.ChildNodes())
                                {
                                    nodes.Enqueue(childNode);
                                }

                                includedNamespaces.Push(null);
                                nodes.Enqueue(null);

                                break;

                            case null:
                                while (includedNamespaces.TryPop(out INamespaceSymbol? removedNamespace) && removedNamespace != null) { }

                                break;
                        }
                    }

                LOOP_DONE:
                    foreach (INamespaceSymbol? @namespace in includedNamespaces)
                    {
                        if (@namespace != null)
                        {
                            namespaces.Add(@namespace);
                        }
                    }
                }
            }
        }

        return new Report()
        {
            IncludedNamespaces = [.. namespaces]
        };
    }
}
