using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Features.ConvertAsserts;

namespace RhinoToNSubstitute.Visitor
{
    public partial class RhinoToNSubstituteVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            node = ConvertMockRepositoryStatements.Convert(node);
            return base.VisitMemberAccessExpression(node);
        }
    }
}
