using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

#pragma warning disable IDE0130
namespace System
{
    namespace Runtime.CompilerServices
    {
        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        internal static class IsExternalInit;

        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        internal class RequiredMemberAttribute : Attribute;

        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
#pragma warning disable CS9113
        internal class CompilerFeatureRequiredAttribute(string feature) : Attribute;
#pragma warning restore CS9113
    }

    namespace Diagnostics.CodeAnalysis
    {
        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        internal class SetsRequiredMembersAttribute : Attribute;

        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Property | AttributeTargets.ReturnValue, AllowMultiple = true, Inherited = false)]
        internal sealed class NotNullIfNotNullAttribute(string parameterName) : Attribute
        {
            public string ParameterName { get; } = parameterName;
        }

        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
        internal sealed class NotNullWhenAttribute(bool returnValue) : Attribute
        {
            public bool ReturnValue { get; } = returnValue;
        }

        [AttributeUsage(AttributeTargets.Parameter, Inherited = false)]
        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        internal sealed class MaybeNullWhenAttribute(bool returnValue) : Attribute
        {
            public bool ReturnValue { get; } = returnValue;
        }

        [ExcludeFromCodeCoverage, DebuggerNonUserCode]
        [AttributeUsage(AttributeTargets.Method | AttributeTargets.Property, Inherited = false, AllowMultiple = true)]
        internal sealed class MemberNotNullWhenAttribute(bool returnValue, params string[] members) : Attribute
        {
            public bool ReturnValue { get; } = returnValue;
            public string[] Members { get; } = members;
        }
    }
}
#pragma warning restore IDE0130