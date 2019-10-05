using System;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace RhinoToNSubstitute.Extensions
{
    static class Extensions
    {
        public static TSyntax FindByName<TSyntax>(
            this SyntaxList<TSyntax> list,
            Func<TSyntax, SyntaxNode> accessor,
            string value) where TSyntax : SyntaxNode
            => list.Single(t => accessor(t).ToString() == value);

        public static void Deconstruct<TNode>(
            this SeparatedSyntaxList<TNode> list,
            out TNode first
        )
            where TNode : SyntaxNode
        {
            first = list[0];
        }


    }
}
