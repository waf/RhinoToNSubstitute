using System.IO;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using RhinoToNSubstitute.Visitor;
using Xunit;

namespace RhinoToNSubstitute.Tests
{
    /// <summary>
    /// Allows for "Snapshot testing" where we have the input and expected output as C# files on disk.
    /// The files' properties should be set as Build Action: "Content"; Copy:"Copy If Newer" to avoid
    /// attempted compilation of these files.
    /// </summary>
    public static class SyntaxSnapshot
    {
        internal static void RunSnapshotTest(string testCategory, string testCase)
        {
            // read input/expected syntax nodes
            var input = ReadCSharpFile(Path.Combine(testCategory, testCase, "Input.cs"));
            var expectedTransformation = ReadCSharpFile(Path.Combine(testCategory, testCase, "Expected.cs"));

            // system under test
            var actualTransformation = new RhinoToNSubstituteVisitor(new Options()).Visit(input);

            var expected = expectedTransformation.ToFullString();
            var actual = actualTransformation.ToFullString();

            Assert.Equal(expected, actual);
        }

        private static SyntaxNode ReadCSharpFile(string filename)
        {
            var source = File.ReadAllText(filename);
            return CSharpSyntaxTree.ParseText(source).GetRoot();
        }
    }
}
