using System.CommandLine.DragonFruit;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using Microsoft.CodeAnalysis.CSharp;
using RhinoToNSubstitute.Visitor;

[assembly: InternalsVisibleTo("RhinoToNSubstitute.Tests")]
namespace RhinoToNSubstitute
{
    public static class Program
    {
        /// <summary>
        /// Convert RhinoMocks code to NSubstitute
        /// </summary>
        /// <param name="projectDir">The directory path of the project to convert.</param>
        public static void Main(DirectoryInfo projectDir)
        {
            if(projectDir == null)
            {
                CommandLine.InvokeMethod(new[] { "--help" }, typeof(Program).GetMethod("Main"));
                return;
            }

            var options = new Options { };  // todo
            var files = Directory.GetFiles(projectDir.FullName, "*.cs", SearchOption.AllDirectories);
            foreach (var file in files)
            {
                var rhinoTree = CSharpSyntaxTree.ParseText(File.ReadAllText(file)).GetRoot();
                var nsubsTree = new RhinoToNSubstituteVisitor(options).Visit(rhinoTree);
                File.WriteAllText(file, nsubsTree.ToFullString(), Encoding.UTF8);
            }
        }
    }
}
