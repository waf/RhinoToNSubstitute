using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RhinoToNSubstitute.Features.ConvertStub
{
    public static class ConvertStub
    {
        internal static InvocationExpressionSyntax Convert(InvocationExpressionSyntax node)
        {
            /* Pattern match `node` for the following structure:
               foo
                 .Stub(f => f.Bar())
                 .Return("MyValue")
             */
            var result = node switch
            {
                InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Expression: InvocationExpressionSyntax
                        {
                            Expression: MemberAccessExpressionSyntax
                            {
                                Name: IdentifierNameSyntax("Stub")
                            } rhinoStubCall,
                            ArgumentList: ArgumentListSyntax
                            (
                                SimpleLambdaExpressionSyntax
                                {
                                    Body: var stubbedExpression
                                }
                            )
                        },
                        Name: SimpleNameSyntax("Return")
                    } rhinoReturnCall
                } => CreateNSubstituteStub(stubbedExpression, rhinoStubCall, rhinoReturnCall, node),
                _ => node,
            };

            return result ?? node;
        }

        private static InvocationExpressionSyntax CreateNSubstituteStub(
            CSharpSyntaxNode stubbedExpression,
            MemberAccessExpressionSyntax rhinoStubCall,
            MemberAccessExpressionSyntax rhinoReturnCall,
            InvocationExpressionSyntax rhinoStubReturnCall)
        {
            // Given foo.Stub(f => f.Bar),
            // foo.Stub is the rhinoStubCall, and f.Bar is the stubbedExpression
            // combine them into foo.Bar
            var stubbedCall = ApplyStubbedExpressionToStubObject(rhinoStubCall, stubbedExpression);

            // rename Return to Returns
            var returnCall = rhinoReturnCall
                .WithName(IdentifierName("Returns"));

            return rhinoStubReturnCall
                .WithExpression(
                    returnCall.WithExpression(stubbedCall)
                );
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
