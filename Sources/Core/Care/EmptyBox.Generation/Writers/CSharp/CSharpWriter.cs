using EmptyBox.Generation.Extensions;
using EmptyBox.Generation.Polyfills;

using Microsoft.CodeAnalysis;

using System;
using System.Collections.Generic;
using System.Linq;

namespace EmptyBox.Generation.Writers.CSharp;

internal class CSharpWriter(string initial = "") : AbstractWriter<CSharpWriter>(initial)
{
    private class TypeDeclarationScope : IDisposable
    {
        private readonly Stack<IDisposable> Scopes = [];

        private bool IsDisposed;

        public TypeDeclarationScope(CSharpWriter writer, INamedTypeSymbol? type)
        {
            if (type != null)
            {
                Stack<INamedTypeSymbol> containers = new(type.EnumerateContainerTypes());
                INamedTypeSymbol? symbol = type;

                do
                {
                    writer.Append("partial ")
                          .AppendTypeKind(symbol.TypeKind)
                          .Append(' ')
                          .Append(symbol.Name)
                          .AppendTypeArguments(symbol.TypeParameters, options: new TypeWriterOptions() { Style = WriterPresentationStyle.Declaration })
                          .AppendLine();

                    Scopes.Push(writer.AppendScope());
                } while (containers.TryPop(out symbol));
            }
        }

        public void Dispose()
        {
            if (IsDisposed)
            {
                throw new ObjectDisposedException(nameof(TypeDeclarationScope));
            }
            else
            {
                IsDisposed = true;

                while (Scopes.TryPop(out IDisposable? scope))
                {
                    scope.Dispose();
                }
            }
        }
    }

    public CSharpWriter AppendAccessibility(Accessibility accessibility)
    {
        Append(accessibility switch
        {
            Accessibility.Internal => "internal",
            Accessibility.Public => "public",
            Accessibility.Private => "private",
            Accessibility.ProtectedAndInternal => "protected internal",
            Accessibility.Protected or _ => "protected",
        });

        return this;
    }

    public CSharpWriter AppendDocumentationInheritance(IMethodSymbol symbol)
    {
        Append("/// <inheritdoc cref=\"");
        AppendMethod(symbol, options: new MethodWriterOptions() { Style = WriterPresentationStyle.Documentation });
        AppendLine("\"/>");

        return this;
    }

    public CSharpWriter AppendDocumentationInheritance(INamedTypeSymbol symbol)
    {
        Append("/// <inheritdoc cref=\"");
        AppendType(symbol, options: new TypeWriterOptions() { Style = WriterPresentationStyle.Documentation });
        AppendLine("\"/>");

        return this;
    }

    public CSharpWriter AppendMethodParameterRefKind(RefKind refKind, MethodParameterWriterOptions options = default)
    {
        switch (refKind)
        {
            case RefKind.RefReadOnlyParameter:
                switch (options.Style)
                {
                    case WriterPresentationStyle.Declaration or WriterPresentationStyle.Documentation: Append("ref readonly "); break;
                    default: Append("ref "); break;
                }

                break;

            case RefKind.Ref: Append("ref "); break;
            case RefKind.Out: Append("out "); break;
            case RefKind.In: Append("in "); break;
        }

        return this;
    }

    public CSharpWriter AppendMethodParameter(IParameterSymbol parameter, MethodParameterWriterOptions options = default)
    {
        switch (options.Style)
        {
            case WriterPresentationStyle.Declaration:
                switch (parameter.ScopedKind)
                {
                    case ScopedKind.ScopedRef or ScopedKind.ScopedValue:
                        Append("scoped ");

                        break;
                }

                if (parameter.IsParams)
                {
                    Append("params ");
                }

                break;
        }

        AppendMethodParameterRefKind(parameter.RefKind, options: options);

        switch (options.Style)
        {
            case WriterPresentationStyle.Declaration:
                if (parameter.IsThis)
                {
                    Append("this ");
                }

                AppendType(parameter.Type, options: new TypeWriterOptions()
                {
                    Style = WriterPresentationStyle.Installation,
                    Presentation = options.TypePresentation,
                    Transformation = options.TypeTransformation
                });

                break;

            case WriterPresentationStyle.Documentation:
                AppendType(parameter.Type, options: new TypeWriterOptions()
                {
                    Style = options.Style,
                    Presentation = options.TypePresentation,
                    Transformation = options.TypeTransformation
                });

                break;
        }

        switch (options.Style)
        {
            case WriterPresentationStyle.Declaration: Append(' '); goto case WriterPresentationStyle.Installation;
            case WriterPresentationStyle.Installation: Append(parameter.Name); break;
        }

        switch (options.Style)
        {
            case WriterPresentationStyle.Declaration:
                if (parameter.HasExplicitDefaultValue)
                {
                    Append(" = ");
                    Append(parameter.ExplicitDefaultValue ?? "default");
                }

                break;
        }

        return this;
    }

    public CSharpWriter AppendMethodReturnParameter(IMethodSymbol method, MethodWriterOptions options = default)
    {
        switch (options.Style)
        {
            case WriterPresentationStyle.Installation when options.Presentation.IsInvocation:
                switch (options.Presentation.ReturnParameterUsage)
                {
                    case MethodReturnParameterUsage.Await: Append("await "); break;
                    case MethodReturnParameterUsage.Reference: Append("ref "); break;
                }

                break;

            case WriterPresentationStyle.Declaration:
                switch (options.Presentation.ReturnParameterUsage)
                {
                    case MethodReturnParameterUsage.Await: Append("async "); break;
                }

                ITypeSymbol returnType = options.Transformation.ReturnParameterTransformation?.TypeSubstitution?.Invoke(method.ReturnType) ?? method.ReturnType;

                if (returnType.SpecialType == SpecialType.System_Void)
                {
                    Append("void ");
                }
                else
                {
                    if (method.ReturnsByRefReadonly)
                    {
                        Append("ref readonly ");
                    }
                    else if (method.ReturnsByRef)
                    {
                        Append("ref ");
                    }

                    AppendType(method.ReturnType, options: new TypeWriterOptions()
                    {
                        Presentation = options.TypePresentation,
                        Transformation = options.Transformation.ReturnParameterTransformation ?? options.TypeTransformation
                    });

                    Append(' ');
                }

                break;
        }

        return this;
    }

    public CSharpWriter AppendMethod(IMethodSymbol method, MethodWriterOptions options = default)
    {
        TypeWriterOptions typeOptions = new()
        {
            Presentation = options.TypePresentation,
            Style = options.Style,
            Transformation = options.TypeTransformation
        };

        AppendMethodReturnParameter(method, options: options);

        switch (options.Style)
        {
            case WriterPresentationStyle.Installation:
                if (method.IsStatic || options.Presentation.IsStatic)
                {
                    AppendType(method.ContainingType, options: typeOptions);
                    Append('.');
                }
                else if (!string.IsNullOrEmpty(options.Presentation.InvocationTarget))
                {
                    Append(options.Presentation.InvocationTarget!);
                    Append('.');
                }

                break;

            case WriterPresentationStyle.Documentation:
                AppendType(method.ContainingType, options: typeOptions);
                Append('.');

                break;
        }

        if (options.Transformation.NameSubstitution != null)
        {
            options.Transformation.NameSubstitution(this, method);
        }
        else if (method.ExplicitInterfaceImplementations.Length > 0)
        {
            Append(method.ExplicitInterfaceImplementations[0].Name);
        }
        else
        {
            Append(method.Name);
        }

        if (options.Transformation.TypeParameterSubstitution != null)
        {
            options.Transformation.TypeParameterSubstitution(this, method.TypeArguments, typeOptions);
        }
        else
        {
            AppendTypeArguments(method.TypeArguments, options: typeOptions);
        }

        switch (options.Style)
        {
            case WriterPresentationStyle.Installation when options.Presentation.IsInvocation:
            case WriterPresentationStyle.Declaration:
            case WriterPresentationStyle.Documentation:
                MethodParameterWriterOptions parameterOptions = new()
                {
                    Style = options.Style,
                    TypePresentation = options.TypePresentation,
                    TypeTransformation = options.TypeTransformation,
                };

                using (var methodArgumentsScope = AppendScope(ScopeBracketFraming.Round, ScopeParameters.NoIndent))
                {
                    if (options.Transformation.ParameterSubstitution != null)
                    {
                        options.Transformation.ParameterSubstitution(this, method.Parameters, parameterOptions);
                    }
                    else
                    {
                        AppendJoin(method.Parameters, (writer, parameter) => writer.AppendMethodParameter(parameter, options: parameterOptions));
                    }
                }

                break;
        }

        return this;
    }

    public CSharpWriter AppendType(ITypeSymbol symbol, IEnumerable<ITypeSymbol>? customTypeArguments = null, TypeWriterOptions options = default)
    {
        if (options.Transformation.TypeSubstitution != null)
        {
            symbol = options.Transformation.TypeSubstitution(symbol);
        }

        switch (symbol)
        {
            case IArrayTypeSymbol array:
                AppendType(array.ElementType, options: options);
                Append('[');
                Append(',', array.Rank - 1);
                Append(']');

                break;

            case IPointerTypeSymbol pointer:
                AppendType(pointer.PointedAtType, options: options);
                Append("*");

                break;

            default:
                switch (symbol)
                {
                    case ITypeParameterSymbol: break;
                    case IErrorTypeSymbol errorType: break;

                    default:
                        if (!options.Presentation.ContainerStyle.HasFlag(TypePresentationContainerStyle.ExcludeNamespace))
                        {
                            Append("global::");
                            Append(symbol.ContainingNamespace);
                            Append('.');
                        }

                        if (!options.Presentation.ContainerStyle.HasFlag(TypePresentationContainerStyle.ExcludeContainingTypes) && symbol.ContainingType != null)
                        {
                            AppendJoin(symbol.EnumerateContainerTypes(),
                                       formatter: (writer, type) => writer.AppendType(type, options: options with
                                       {
                                           Presentation = options.Presentation with
                                           {
                                               ContainerStyle = TypePresentationContainerStyle.ExcludeNamespace | TypePresentationContainerStyle.ExcludeContainingTypes
                                           }
                                       }),
                                       separator: ".");
                            Append('.');
                        }
                        break;
                }

                if (options.Transformation.NameSubstitution != null)
                {
                    options.Transformation.NameSubstitution(this, symbol);
                }
                else
                {
                    Append(symbol.Name);
                }

                IEnumerable<ITypeSymbol>? typeArguments = customTypeArguments;

                if (typeArguments == null && symbol is INamedTypeSymbol namedTypeSymbol)
                {
                    typeArguments = options.Style switch
                    {
                        WriterPresentationStyle.Documentation => namedTypeSymbol.TypeParameters,
                        _ => namedTypeSymbol.TypeArguments
                    };
                }

                if (typeArguments != null)
                {
                    AppendTypeArguments(typeArguments, options: options);
                }

                break;
        }

        if (!symbol.IsValueType && symbol.NullableAnnotation == NullableAnnotation.Annotated)
        {
            Append('?');
        }

        return this;
    }

    public CSharpWriter AppendTypeKind(TypeKind kind)
    {
        Append(kind switch
        {
            TypeKind.Class => "class",
            TypeKind.Delegate => "delegate",
            TypeKind.Enum => "enum",
            TypeKind.Interface => "interface",
            TypeKind.Structure => "struct",
            _ => "UNSUPPORTED_TYPE"
        });

        return this;
    }

    public CSharpWriter AppendTypeParameterConstraint(ITypeParameterSymbol parameter, TypeWriterOptions options = default)
    {
        Append($"where ");
        AppendType(parameter, options: options);
        Append(" : ");
        bool separate = false;

        (_, separate) = parameter switch
        {
            ITypeParameterSymbol typeParameter when typeParameter.HasReferenceTypeConstraint => (Append("class"), true),
            ITypeParameterSymbol typeParameter when typeParameter.HasUnmanagedTypeConstraint => (Append("unmanaged"), true),
            ITypeParameterSymbol typeParameter when typeParameter.HasValueTypeConstraint => (Append("struct"), true),
            ITypeParameterSymbol typeParameter when typeParameter.HasNotNullConstraint => (Append("notnull"), true),
            _ => (this, separate),
        };

        foreach (ITypeSymbol symbol in parameter.ConstraintTypes)
        {
            if (separate)
            {
                Append(", ");
            }

            AppendType(symbol, options: options);
            separate = true;
        }

        if (parameter.HasConstructorConstraint)
        {
            if (separate)
            {
                Append(", ");
            }

            Append("new()");
            separate = true;
        }

        if (parameter.AllowsRefLikeType)
        {
            if (separate)
            {
                Append(", ");
            }

            Append("allows ref struct");
        }

        return this;
    }

    public CSharpWriter AppendTypeParameterConstraints(IEnumerable<ITypeParameterSymbol> typeParameters, TypeWriterOptions options = default)
    {
        using (var constraintsScope = AppendScope(ScopeBracketFraming.None, ScopeParameters.Deferred))
        {
            AppendJoin(typeParameters.Where(TypeParameterSymbolExtensions.HasAnyConstraint),
                       formatter: (writer, constraint) => writer.AppendTypeParameterConstraint(constraint, options: options),
                       separator: NewLine);
        }

        return this;
    }

    public CSharpWriter AppendTypeArgument(ITypeSymbol argument, TypeWriterOptions options = default)
    {
        if (argument is ITypeParameterSymbol parameter && options.Style == WriterPresentationStyle.Declaration)
        {
            switch (parameter.Variance)
            {
                case VarianceKind.In: Append("in "); break;
                case VarianceKind.Out: Append("out "); break;
            }
        }

        AppendType(argument, options: options);

        return this;
    }

    public CSharpWriter AppendTypeArguments(IEnumerable<ITypeSymbol> arguments, TypeWriterOptions options = default)
    {
        ScopeBracketFraming framing = options.Style switch
        {
            WriterPresentationStyle.Documentation => ScopeBracketFraming.Curly,
            _ => ScopeBracketFraming.Angle
        };

        using (var typeArgumentsScope = AppendScope(framing, ScopeParameters.NoIndent | ScopeParameters.Deferred))
        {
            AppendJoin(arguments, (writer, parameter) => writer.AppendTypeArgument(parameter, options: options));
        }

        return this;
    }

    public IDisposable AppendTypeDeclarationScope(INamedTypeSymbol? type)
    {
        return new TypeDeclarationScope(this, type);
    }
}
