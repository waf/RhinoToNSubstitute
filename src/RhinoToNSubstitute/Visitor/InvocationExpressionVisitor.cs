using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Features.ConvertStub;

namespace RhinoToNSubstitute.Visitor
{
    partial class RhinoToNSubstituteVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            node = ConvertStub.Convert(node);
            return base.VisitInvocationExpression(node);
        }
    }
}
