using System;

namespace EmptyBox.Generation.Writers.CSharp;

[Flags]
internal enum TypePresentationContainerStyle
{
    All = 0b00,
    ExcludeNamespace = 0b01,
    ExcludeContainingTypes = 0b11,
}
