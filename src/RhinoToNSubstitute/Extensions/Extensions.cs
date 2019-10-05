using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace RhinoToNSubstitute.Extensions
{
    static class Extensions
    {
        public static TSyntax FindByName<TSyntax>(
            this SyntaxList<TSyntax> list,
            Func<TSyntax, SyntaxNode> accessor,
            string value) where TSyntax : SyntaxNode
            => list.Single(t => accessor(t).ToString() == value);

        public static void Deconstruct(this ArgumentListSyntax arguments, out ExpressionSyntax first)
        {
            first = arguments.Arguments[0].Expression;
        }
        public static void Deconstruct(this ArgumentListSyntax arguments, out ExpressionSyntax first, out ExpressionSyntax second)
        {
            first = arguments.Arguments[0].Expression;
            second = arguments.Arguments[1].Expression;
        }
        public static void Deconstruct(this ArgumentListSyntax arguments, out ExpressionSyntax first, out ExpressionSyntax second, out ExpressionSyntax third)
        {
            first = arguments.Arguments[0].Expression;
            second = arguments.Arguments[1].Expression;
            third = arguments.Arguments[2].Expression;
        }

        public static void Deconstruct(
            this SimpleNameSyntax identifier,
            out string name
        )
        {
            name = identifier.Identifier.ValueText;
        }

        public static void Deconstruct(
            this ExpressionSyntax expr,
            out string name
        )
        {
            name = expr.ToFullString();
        }
    }
}
