using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Extensions;
using static Microsoft.CodeAnalysis.CSharp.SyntaxFactory;

namespace RhinoToNSubstitute.Features.AddRequiredUsings
{
    static class AddRequiredUsings
    {
        public static RequiredUsings GetDefaultState() =>
            new RequiredUsings();

        public static CompilationUnitSyntax AddUsings(CompilationUnitSyntax node, Options options)
        {
            // Remember leading trivia (e.g. license header comments) so we can restore
            // it later. For some reason, manipulating the usings can remove it.
            var comment = node.GetLeadingTrivia();
            var treeWithTriviaTrimmed = node.WithoutLeadingTrivia();

            // add any usings that were required when visiting the tree
            var usingsToRemove = treeWithTriviaTrimmed.Usings.FindByName(u => u.Name, "Rhino.Mocks");
            var usingsToAdd = CreateUsing("NSubstitute");
            var treeWithUsings = AddUsingsToCompilationUnit(treeWithTriviaTrimmed, usingsToRemove, usingsToAdd);

            // restore the leading trivia to the new syntax tree. 
            var treeWithTriviaRestored = treeWithUsings.WithLeadingTrivia(comment);

            return treeWithTriviaRestored;
        }

        private static UsingDirectiveSyntax CreateUsing(string identifier) =>
            UsingDirective(IdentifierName(identifier))
                .NormalizeWhitespace()
                .WithTrailingTrivia(Whitespace(Environment.NewLine));

        /// <summary>
        /// Adds the usings to the compilation unit, adding a blank line after the last using
        /// </summary>
        private static CompilationUnitSyntax AddUsingsToCompilationUnit(CompilationUnitSyntax nsubsTree, UsingDirectiveSyntax usingToRemove, UsingDirectiveSyntax usingToAdd)
        {
            // ensure the last using has a blank line after it.
            usingToAdd = usingToAdd.WithTrailingTrivia(Whitespace(Environment.NewLine + Environment.NewLine));
            var existingUsings = nsubsTree.Usings.ToArray();

            // remove the existing usings, trim the leading trivia, then add all usings back in with the trailing space.
            // this handles the cases of:
            //   - no existing usings
            //   - one existing using that will be replaced
            //   - one existing using that will be appended to
            return nsubsTree
                .RemoveNodes(existingUsings, SyntaxRemoveOptions.KeepNoTrivia)
                .WithoutLeadingTrivia()
                .WithUsings(List(existingUsings.Except(new[] { usingToRemove }).Append(usingToAdd).ToArray()));
        }
    }
}
