using System;

namespace EmptyBox.Generation.Writers;

[Flags]
internal enum ScopeParameters
{
    Default = 0,
    NoIndent = 0b01,
    Deferred = 0b10
}
