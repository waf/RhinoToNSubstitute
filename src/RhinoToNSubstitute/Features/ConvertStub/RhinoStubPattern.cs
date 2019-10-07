using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Extensions;

namespace RhinoToNSubstitute.Features.ConvertStub
{
    /// <summary>
    /// Pattern match `node` for the following structure:
    ///
    ///   foo
    ///     .Stub(f => f.Bar())
    ///
    /// and convert it to:
    ///
    ///   foo
    ///     .Bar()
    ///
    /// </summary>
    class RhinoStubPattern
    {
        private readonly MemberAccessExpressionSyntax root;
        private readonly MemberAccessExpressionSyntax rhinoStubMethod;
        private readonly CSharpSyntaxNode stubbedExpression;

        private RhinoStubPattern(MemberAccessExpressionSyntax root, MemberAccessExpressionSyntax rhinoStubMethod, CSharpSyntaxNode stubbedExpression)
        {
            this.root = root;
            this.rhinoStubMethod = rhinoStubMethod;
            this.stubbedExpression = stubbedExpression;
        }

        public static RhinoStubPattern? Match(MemberAccessExpressionSyntax invocation)
        {
            return invocation switch
            {
                MemberAccessExpressionSyntax
                {
                    Expression: InvocationExpressionSyntax
                    {
                        Expression: MemberAccessExpressionSyntax
                        {
                            Name: IdentifierNameSyntax("Stub")
                        } rhinoStubMethod,
                        ArgumentList: ArgumentListSyntax
                        (
                            SimpleLambdaExpressionSyntax
                            {
                                Body: var stubbedExpression
                            }
                        )
                    }
                } => new RhinoStubPattern(invocation, rhinoStubMethod, stubbedExpression),
                _ => null
            };
        }

        public MemberAccessExpressionSyntax Convert()
        {
            // Given foo.Stub(f => f.Bar),
            // foo.Stub is the rhinoStubCall, and f.Bar is the stubbedExpression
            // combine them into foo.Bar
            var nsubsStub = ApplyStubbedExpressionToStubObject(rhinoStubMethod, stubbedExpression);

            return root
                .WithExpression(nsubsStub);
        }

        private static ExpressionSyntax ApplyStubbedExpressionToStubObject(
            MemberAccessExpressionSyntax rhinoStubCall,
            CSharpSyntaxNode stubbedExpression)
        {
            var applied = stubbedExpression switch
            {
                // stubbed property
                MemberAccessExpressionSyntax memberAccess =>
                    ApplyStubbedMemberToStubObject(rhinoStubCall, memberAccess),

                // stubbed method
                InvocationExpressionSyntax
                { Expression: MemberAccessExpressionSyntax memberAccess } invocation =>
                    ApplyStubbedMethodToStubObject(invocation, rhinoStubCall, memberAccess),
                _ => throw new NotImplementedException("Unknown pattern: " + stubbedExpression.ToFullString())
            };

            return applied.WithTriviaFrom(rhinoStubCall.Parent);
        }

        private static ExpressionSyntax ApplyStubbedMemberToStubObject(
            MemberAccessExpressionSyntax rhinoStubCall,
            MemberAccessExpressionSyntax memberAccess) =>
            memberAccess
                .WithExpression(rhinoStubCall.Expression)
                .WithOperatorToken(rhinoStubCall.OperatorToken);

        private static ExpressionSyntax ApplyStubbedMethodToStubObject(
            InvocationExpressionSyntax invocation,
            MemberAccessExpressionSyntax rhinoStubCall,
            MemberAccessExpressionSyntax memberAccess) =>
            invocation
                .WithExpression(ApplyStubbedMemberToStubObject(rhinoStubCall, memberAccess));
    }
}
