using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Features.ConvertMockRepository;
using RhinoToNSubstitute.Features.ConvertStub;

namespace RhinoToNSubstitute.Visitor
{
    partial class RhinoToNSubstituteVisitor : CSharpSyntaxRewriter
    {
        public override SyntaxNode VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            node = ConvertMockRepository.Convert(node);
            node = ConvertStub.Convert(node);
            return base.VisitMemberAccessExpression(node);
        }
    }
}
