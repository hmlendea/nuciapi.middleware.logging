using System;
using System.IO;
using System.Linq;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

using NUnit.Framework;

namespace NuciAPI.Middleware.Logging.ArchitectureTests
{
    [TestFixture]
    public sealed class SourceFileConventionTests : ArchitectureTestsFixture
    {
        [Test]
        public void GivenSourceFiles_WhenTypeDeclarationAndFilenameConventionsInspected_ThenEachFileDeclaresExactlyOneMatchingType()
        {
            var sourceFiles = Directory.GetFiles(RepositoryRoot, "*.cs", SearchOption.AllDirectories)
                .Where(IsEligibleHandwrittenSourceFile)
                .ToList();

            Assert.That(sourceFiles.Count, Is.GreaterThan(0), "No handwritten C# source files were found for source-file convention checks.");

            foreach (var filePath in sourceFiles)
            {
                string sourceText = File.ReadAllText(filePath);
                SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(sourceText, path: filePath);
                SyntaxNode root = syntaxTree.GetRoot();

                BaseTypeDeclarationSyntax[] typeDeclarations = root
                    .DescendantNodes()
                    .OfType<BaseTypeDeclarationSyntax>()
                    .ToArray();

                string fileName = Path.GetFileNameWithoutExtension(filePath);

                Assert.Multiple(() =>
                {
                    Assert.That(
                        typeDeclarations.Length,
                        Is.EqualTo(1),
                        $"File '{filePath}' declares {typeDeclarations.Length} types; exactly one type per handwritten .cs file is required.");

                    Assert.That(
                        typeDeclarations[0].Identifier.Text,
                        Is.EqualTo(fileName).Using(System.StringComparer.Ordinal),
                        $"Type '{typeDeclarations[0].Identifier.Text}' in file '{filePath}' does not match the file name '{fileName}' (case-sensitive).");
                });
            }
        }

        private static bool IsEligibleHandwrittenSourceFile(string filePath)
        {
            string[] segments = filePath.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);

            if (segments.Any(segment => segment.Equals("bin", StringComparison.Ordinal) || segment.Equals("obj", StringComparison.Ordinal)))
            {
                return false;
            }

            if (Path.GetFileName(filePath).EndsWith(".g.cs", StringComparison.Ordinal) || Path.GetFileName(filePath).EndsWith(".generated.cs", StringComparison.Ordinal))
            {
                return false;
            }

            return true;
        }
    }
}
