using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Immutable;

namespace EmptyBox.Generation.Proxies.Construction.Syntaxes.CSharp;

internal abstract partial class MethodProxyBuilder
{
    protected sealed class MethodParameterTargetPrependSubstitution
    {
        public required bool IsExtension { get; init; }
        public required bool Await { get; init; }
        public required RefKind RefKind { get; init; }
        public required string TargetName { get; init; }
        public required INamedTypeSymbol TargetType { get; init; }

        public void Prepend(CSharpWriter writer, ImmutableArray<IParameterSymbol> parameters, MethodParameterWriterOptions options)
        {
            writer.AppendMethodParameterRefKind(RefKind);

            switch (options.Style)
            {
                case WriterPresentationStyle.Declaration:
                    if (IsExtension)
                    {
                        writer.Append("this ");
                    }

                    writer.AppendType(TargetType, options: new TypeWriterOptions()
                    {
                        Style = WriterPresentationStyle.Installation,
                        Presentation = options.TypePresentation,
                        Transformation = options.TypeTransformation
                    });

                    break;

                case WriterPresentationStyle.Documentation:
                    writer.AppendType(TargetType, options: new TypeWriterOptions()
                    {
                        Style = options.Style,
                        Presentation = options.TypePresentation,
                        Transformation = options.TypeTransformation
                    });

                    break;

                case WriterPresentationStyle.Installation when Await:
                    writer.Append("await ");

                    break;
            }

            switch (options.Style)
            {
                case WriterPresentationStyle.Declaration:
                    writer.Append(' ');

                    goto case WriterPresentationStyle.Installation;

                case WriterPresentationStyle.Installation:
                    writer.Append(TargetName);

                    break;
            }

            if (parameters.Length > 0)
            {
                writer.Append(", ")
                      .AppendJoin(parameters, (writer, parameter) => writer.AppendMethodParameter(parameter, options: options));
            }
        }
    }
}