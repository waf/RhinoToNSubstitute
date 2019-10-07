using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RhinoToNSubstitute.Features.ConvertAsserts
{
    class RhinoMockRepositoryPattern
    {
        private static readonly IReadOnlyCollection<string> StubMockExpressions = new[]
        {
            "GenerateStub",
            "GenerateMock",
        };
        private readonly MemberAccessExpressionSyntax root;
        private readonly GenericNameSyntax member;

        private RhinoMockRepositoryPattern(MemberAccessExpressionSyntax root, GenericNameSyntax member)
        {
            this.root = root;
            this.member = member;
        }

        public static RhinoMockRepositoryPattern? Match(MemberAccessExpressionSyntax node) =>
            node switch
            {
                MemberAccessExpressionSyntax
                {
                    Expression: ExpressionSyntax("MockRepository"),
                    Name: GenericNameSyntax member
                } when IsRhinoStubMethod(member) => new RhinoMockRepositoryPattern(node, member),
                _ => null
            };

        private static bool IsRhinoStubMethod(GenericNameSyntax member) =>
            StubMockExpressions.Contains(member.Identifier.ToString());

        public MemberAccessExpressionSyntax Convert() =>
            root
                .WithExpression(IdentifierName("Substitute"))
                .WithName(GenericName("For").WithTypeArgumentList(member.TypeArgumentList));
    }
}
