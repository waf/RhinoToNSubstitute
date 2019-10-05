using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Extensions;
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

        internal static MemberAccessExpressionSyntax Convert(MemberAccessExpressionSyntax node) =>
            node switch
            {
                MemberAccessExpressionSyntax
                {
                    Expression: ExpressionSyntax("MockRepository"),
                    Name: GenericNameSyntax genericName
                } when IsRhinoStubMethod(genericName) => BuildSubstituteForExpression(node, genericName),
                _ => node
            };

        private static bool IsRhinoStubMethod(GenericNameSyntax genericName) =>
            StubMockExpressions.Contains(genericName.Identifier.ToString());

        private static MemberAccessExpressionSyntax BuildSubstituteForExpression(
            MemberAccessExpressionSyntax node,
            GenericNameSyntax genericName) =>
            node
                .WithExpression(IdentifierName("Substitute"))
                .WithName(GenericName("For").WithTypeArgumentList(genericName.TypeArgumentList));
    }
}
