using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RhinoToNSubstitute.Features.ConvertAsserts
{
    static class ConvertMockRepositoryStatements
    {
        private static readonly IReadOnlyCollection<string> StubMockExpressions = new[]
        {
            "GenerateStub",
            "GenerateMock",
        };

        internal static MemberAccessExpressionSyntax Convert(MemberAccessExpressionSyntax node)
        {
            if (node.Expression.ToString() != "MockRepository"
                || !StubMockExpressions.Contains(node.Name.Identifier.ToString())
                || !(node.Name is GenericNameSyntax genericName))
            {
                return node;
            }
            return node
                .WithExpression(IdentifierName("Substitute"))
                .WithName(GenericName("For").WithTypeArgumentList(genericName.TypeArgumentList));
        }
    }
}
