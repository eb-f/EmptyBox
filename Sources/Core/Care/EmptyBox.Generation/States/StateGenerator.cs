using EmptyBox.Generation.Extensions;
using EmptyBox.Generation.Writers;
using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace EmptyBox.Generation.States;

[Generator]
public class StateGenerator : IIncrementalGenerator
{
    private static void Generate(SourceProductionContext context, Compilation compilation, INamedTypeSymbol declaredType)
    {
        INamedTypeSymbol? qualifierInterface = compilation.GetTypeByMetadataName("EmptyBox.Presentation.Permissions.IQualifier");
        INamedTypeSymbol? compilerGeneratedAttribute = compilation.GetTypeByMetadataName("System.CodeDom.Compiler.GeneratedCodeAttribute");
        INamedTypeSymbol? stateAttribute = compilation.GetTypeByMetadataName("EmptyBox.Construction.Machines.StateAttribute");

        CSharpWriter writer = new();
        bool written = false;

        if (declaredType.TypeKind == TypeKind.Interface && declaredType.AllInterfaces.Contains(qualifierInterface))
        {
            ImmutableArray<AttributeData> attributes = declaredType.GetAttributes();

            if (attributes.Any(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, stateAttribute)))
            {
                static string SubstituteName(ITypeSymbol type)
                {
                    return type.Name[1..];
                }

                TypeWriterOptions interfaceToClassOptions = new()
                {
                    Presentation = new TypePresentationOptions()
                    {
                        ContainerStyle = TypePresentationContainerStyle.ExcludeContainingTypes
                    },
                    Style = WriterPresentationStyle.Declaration,
                    Transformation = new TypeTransformationOptions()
                    {
                        NameSubstitution = (writer, type) =>
                        {
                            if (SymbolEqualityComparer.Default.Equals(type, declaredType))
                            {
                                writer.Append(SubstituteName(type));
                            }
                            else
                            {
                                writer.Append(type.Name);
                            }
                        }
                    }
                };

                AttributeData attribute = attributes.First(x => SymbolEqualityComparer.Default.Equals(x.AttributeClass, stateAttribute));

                ImmutableArray<INamedTypeSymbol> interfaces = [declaredType, .. declaredType.AllInterfaces];
                int offset = 0;
                written = true;
                string @namespace = declaredType.ContainingNamespace.ToDisplayString();
                writer.Append("namespace ")
                      .Append(@namespace)
                      .AppendLine(";")
                      .AppendLine();

                //GeneratedSymbolCache.Add(declaredType.ContainingAssembly, new GeneratedTypeInfo()
                //{
                //    Container = (INamespaceOrTypeSymbol?)declaredType.ContainingType ?? declaredType.ContainingNamespace,
                //    Name = SubstituteName(declaredType),
                //});

                using (var containerTypesScope = writer.AppendTypeDeclarationScope(declaredType.ContainingType))
                {
                    writer.AppendAccessibility(declaredType.DeclaredAccessibility)
                          .Append(" partial class ")
                          .AppendType(declaredType, options: interfaceToClassOptions)
                          .Append(" : ")
                          .AppendType(declaredType)
                          .AppendLine()
                          .AppendTypeParameterConstraints(declaredType.TypeParameters);

                    using (var generatedTypeScope = writer.AppendScope())
                    {
                        ConcurrentDictionary<INamedTypeSymbol, List<IPropertySymbol>> reassignable = [];

                        foreach (INamedTypeSymbol @interface in interfaces)
                        {
                            foreach (IPropertySymbol property in @interface.GetMembers().OfType<IPropertySymbol>())
                            {
                                if (property.GetMethod != null)
                                {
                                    writer.Append("public ")
                                          .AppendType(property.Type)
                                          .Append(' ')
                                          .Append(property.Name)
                                          .Append(' ');

                                    using (var generatedPropertyScope = writer.AppendScope(indent: ScopeParameters.NoIndent))
                                    {
                                        writer.Append(" get; ");

                                        if (property.SetMethod != null)
                                        {
                                            if (property.SetMethod.IsInitOnly)
                                            {
                                                writer.Append("init; ");
                                            }
                                            else
                                            {
                                                if (property.SetMethod.DeclaredAccessibility == Accessibility.Public)
                                                {
                                                    reassignable.GetOrAdd(@interface, static _ => []).Add(property);
                                                }

                                                writer.Append("set; ");
                                            }
                                        }
                                    }

                                    writer.AppendLine();
                                }
                            }
                        }

                        writer.AppendLine();

                        writer.Append(' ', offset).AppendLine($"void EmptyBox.Construction.Machines.IState.Map<S>(S state)");

                        using (var mapMethodScope = writer.AppendScope())
                        {
                            foreach (var @interface in interfaces)
                            {
                                if (reassignable.TryGetValue(@interface, out List<IPropertySymbol>? properties))
                                {
                                    using (var reassignableInterfaceScope = writer.AppendScope())
                                    {
                                        writer.Append("if (state is ")
                                              .AppendType(@interface)
                                              .AppendLine(" variant)");

                                        using (var reassignScope = writer.AppendScope())
                                        {
                                            writer.AppendType(@interface)
                                                  .AppendLine(" @this = this;");

                                            foreach (var property in properties)
                                            {
                                                writer.AppendLine($"variant.{property.Name} = @this.{property.Name};");
                                            }
                                        }
                                    }

                                    writer.AppendLine();
                                }
                            }
                        }
                    }
                }
            }
        }

        if (written)
        {
            context.AddSource($"{declaredType.ContainingNamespace}.{declaredType.MetadataName}.cs", writer.ToString());
        }
    }

    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var typeProvider = context.SyntaxProvider
                                  .CreateSyntaxProvider(static (node, _) => node is TypeDeclarationSyntax typeDeclaration && typeDeclaration.BaseList?.Types.Count > 0,
                                                        static (context, token) => (Declaration: (TypeDeclarationSyntax)context.Node, Symbol: (INamedTypeSymbol)context.SemanticModel.GetDeclaredSymbol(context.Node, token)!))
                                  .Combine(context.CompilationProvider)
                                  .Select(static (tuple, _) => (tuple.Left.Declaration, tuple.Left.Symbol, Compilation: tuple.Right));

        context.RegisterSourceOutput(typeProvider, static (context, tuple) => Generate(context, tuple.Compilation, tuple.Symbol));
    }
}
