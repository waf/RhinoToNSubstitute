using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Features.AddRequiredUsings;

namespace RhinoToNSubstitute.Visitor
{
    partial class RhinoToNSubstituteVisitor : CSharpSyntaxRewriter
    {
        private readonly Options options;

        public RhinoToNSubstituteVisitor(Options options)
        {
            this.options = options;
        }

        public override SyntaxNode VisitCompilationUnit(CompilationUnitSyntax node)
        {
            var xunitTree = (CompilationUnitSyntax)base.VisitCompilationUnit(node);
            var withUsings = AddRequiredUsings.AddUsings(xunitTree, options);
            return withUsings;
        }
    }
}
