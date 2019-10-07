using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RhinoToNSubstitute.Features.ConvertStub
{
    /// <summary>
    /// Convert Rhino's `Return` method into NSubstitute's `Returns` method.
    /// </summary>
    class RhinoReturnPattern
    {
        private readonly InvocationExpressionSyntax root;
        private readonly MemberAccessExpressionSyntax rhinoReturnCall;

        private RhinoReturnPattern(InvocationExpressionSyntax root, MemberAccessExpressionSyntax rhinoReturnCall)
        {
            this.root = root;
            this.rhinoReturnCall = rhinoReturnCall;
        }

        public InvocationExpressionSyntax Convert()
        {
            // rename Return to Returns
            var nsubsReturnsCall = rhinoReturnCall
                .WithName(IdentifierName("Returns"));

            return root.ReplaceNode(rhinoReturnCall, nsubsReturnsCall);
        }

        public static RhinoReturnPattern? Match(InvocationExpressionSyntax invocation)
        {
            return invocation switch
            {
                InvocationExpressionSyntax
                {
                    Expression: MemberAccessExpressionSyntax
                    {
                        Expression: InvocationExpressionSyntax
                        {
                            // match for Stub as a rough heuristic to make
                            // sure that we're converting an Rhino `Return` method
                            // and not some other method called `Return`
                            Expression: MemberAccessExpressionSyntax
                            {
                                Name: IdentifierNameSyntax("Stub")
                            }
                        },
                        Name: SimpleNameSyntax("Return")
                    } rhinoReturnCall
                } => new RhinoReturnPattern(invocation, rhinoReturnCall),
                _ => null
            };
        }
    }
}
