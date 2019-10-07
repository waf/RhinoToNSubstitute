using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using RhinoToNSubstitute.Features.ConvertAsserts;

namespace RhinoToNSubstitute.Features.ConvertMockRepository
{
    class ConvertMockRepository
    {
        internal static MemberAccessExpressionSyntax Convert(MemberAccessExpressionSyntax node) =>
            RhinoMockRepositoryPattern.Match(node)?.Convert()
            ?? node;
    }
}
