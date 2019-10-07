using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RhinoToNSubstitute.Features.ConvertStub
{
    static class ConvertStub
    {
        internal static InvocationExpressionSyntax Convert(InvocationExpressionSyntax node)
        {
            return RhinoReturnPattern.Match(node)?.Convert()
                ?? node;
        }

        internal static MemberAccessExpressionSyntax Convert(MemberAccessExpressionSyntax node)
        {
            return RhinoStubPattern.Match(node)?.Convert()
                ?? node;
        }
    }
}
