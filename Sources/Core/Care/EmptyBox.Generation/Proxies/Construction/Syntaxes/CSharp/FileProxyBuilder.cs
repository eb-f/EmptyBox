using EmptyBox.Generation.Extensions;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal sealed partial class FileProxyBuilder
{
    public required TypeProxyBuilder TypeBuilder { get; init; }

    public void Build(SourceProductionContext productionContext, Context fileContext)
    {
        CSharpWriter writer = new();

        writer.AppendLine("#nullable enable");
        writer.AppendLine();

        foreach (INamespaceSymbol @namespace in fileContext.IncludedNamespaces)
        {
            writer.Append("using ")
                  .Append(@namespace.ToDisplayString())
                  .AppendLine(';');
        }

        writer.AppendLine();

        ImmutableArray<INamedTypeSymbol> typeImports = fileContext.Type.Report.Target.Symbol.TypeKind switch
        {
            TypeKind.Interface => fileContext.Type.Report.Target.Symbol.AllInterfaces,
            TypeKind.Class => [.. fileContext.Type.Report.Target.Symbol.EnumerateBaseTypes()],
            _ => []
        };

        foreach (INamedTypeSymbol typeImport in typeImports)
        {
            if (typeImport.EnumerateAllTypeArguments().All(static argument => argument is not ITypeParameterSymbol) && typeImport.GetTypeMembers().Length > 0)
            {
                writer.Append("using static ")
                      .AppendType(typeImport)
                      .AppendLine(';');
            }
        }

        writer.AppendLine();

        writer.Append("namespace ")
              .Append(fileContext.Type.Report.Target.Symbol.ContainingNamespace.ToDisplayString())
              .AppendLine(";")
              .AppendLine();

        TypeBuilder.Build(fileContext.Type, writer);

        productionContext.AddSource($"{fileContext.Type.Report.Target.Symbol.ContainingNamespace}.{fileContext.Type.Report.Target.Symbol.MetadataName}.{TypeBuilder.Name}.cs", writer.ToString());
    }
}
