using EmptyBox.Generation.Writers.CSharp;

using Microsoft.CodeAnalysis;

using System.Collections.Generic;
using System.Linq;

namespace EmptyBox.Generation.Abstractions.Construction.Syntaxes.CSharp;

internal partial class OptimizedExceptionThrowBuilder : IBuilder<OptimizedExceptionThrowBuilder.Context, CSharpWriter>
{
    public static OptimizedExceptionThrowBuilder Instance { get; } = new();

    private OptimizedExceptionThrowBuilder() { }

    public void Build(Context context, CSharpWriter writer)
    {
        if (context.ReturnType != null)
        {
            writer.Append("return ");
        }

        MethodWriterOptions exceptionThrowWritingOptions = new()
        {
            Presentation = new MethodPresentationOptions()
            {
                IsInvocation = true,
                ReturnParameterUsage = context.ReturnParameterUsage,
            },
            Transformation = new MethodTransformationOptions()
            {
                ParameterSubstitution = (writer, @params, _) =>
                {
                    if (context.MessageWriter != null)
                    {
                        writer.Append(@params[0].Name)
                              .Append(": ");
                        context.MessageWriter(writer);
                    }

                    if (context.InnerExceptionWriter != null)
                    {
                        writer.Append(@params[1].Name)
                              .Append(": ");
                        context.InnerExceptionWriter(writer);
                    }
                }
            },
            TypePresentation = context.TypePresentation,
            TypeTransformation = context.TypeTransformation
        };

        string throwMethodName = context.ReturnParameterUsage switch
        {
            MethodReturnParameterUsage.Reference => "ThrowRef",
            _ => "Throw"
        };

        IEnumerable<IMethodSymbol> methods = context.Symbols.Base.IException.GetMembers().OfType<IMethodSymbol>();
        IMethodSymbol throwMethod;

        if (context.ReturnType != null)
        {
            throwMethod = methods.First(method => method.Name == throwMethodName
                                               && method.TypeParameters.Length == 2
                                               && method.Parameters.Length == 2)
                                 .Construct(context.Target, context.ReturnType);
        }
        else
        {
            throwMethod = methods.First(static method => method.Name == "Throw"
                                                      && method.TypeParameters.Length == 1
                                                      && method.Parameters.Length == 2)
                                 .Construct(context.Target);
        }

        writer.AppendMethod(throwMethod, exceptionThrowWritingOptions)
              .Append(';');
    }
}
