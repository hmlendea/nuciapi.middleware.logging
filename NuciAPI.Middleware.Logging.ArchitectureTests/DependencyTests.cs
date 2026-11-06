using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Xml.Linq;

using ArchUnitNET.Domain;
using ArchUnitNET.Fluent;
using ArchUnitNET.Loader;
using ArchUnitNET.NUnit;

using NUnit.Framework;

using static ArchUnitNET.Fluent.ArchRuleDefinition;

namespace NuciAPI.Middleware.Logging.ArchitectureTests
{
    [TestFixture]
    public sealed class DependencyTests : ArchitectureTestsFixture
    {
        private static readonly string[] ForbiddenPackagePrefixes =
        [
            "Microsoft.EntityFrameworkCore",
            "Amazon.DynamoDBv2",
            "MediatR",
            "Dapper",
            "Npgsql",
            "MongoDB.Driver",
        ];

        private const string ExpectedTargetFramework = "net10.0";
        private const string ImplicitUsingsExpectedValue = "disable";
        private const string LangVersionExpectedValue = "latest";
        private const string NullableExpectedValue = "enable";
        private const string IsPackableExpectedValue = "true";

        private static readonly (string Property, string ExpectedValue)[] RequiredProjectProperties =
        [
            ("TargetFramework", ExpectedTargetFramework),
            ("ImplicitUsings", ImplicitUsingsExpectedValue),
            ("LangVersion", LangVersionExpectedValue),
            ("Nullable", NullableExpectedValue),
            ("IsPackable", IsPackableExpectedValue),
        ];

        [Test]
        public void GivenProductionAssembly_WhenInspectedForInternalsVisibleTo_ThenNoneAreDeclared()
        {
            System.Reflection.Assembly productionAssembly = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName("NuciAPI.Middleware.Logging"));

            var attributes = System.Attribute.GetCustomAttributes(productionAssembly, typeof(InternalsVisibleToAttribute));
            Assert.That(attributes, Is.Empty);
        }

        [Test]
        public void GivenProductionProject_WhenPackageReferencesAreInspected_ThenNoPersistenceInfrastructurePackagesAreDeclared()
        {
            XDocument document = XDocument.Load(ProductionProjectFile);

            var declaredPackages = document
                .Descendants()
                .Where(element => element.Name.LocalName == "PackageReference")
                .Select(element => element.Attribute("Include")!.Value)
                .ToList();

            Assert.That(declaredPackages.Count, Is.GreaterThan(0), "Expected to inspect declared production packages.");

            foreach (var package in declaredPackages)
            {
                Assert.That(ForbiddenPackagePrefixes.Any(prefix => package.StartsWith(prefix, StringComparison.Ordinal)), Is.False, $"Production package '{package}' references forbidden persistence infrastructure.");
            }
        }

        [Test]
        public void GivenProductionProject_WhenProjectReferencesAreInspected_ThenOnlyProductionProjectsAreReferenced()
        {
            var projectReferences = XDocument.Load(ProductionProjectFile)
                .Descendants()
                .Where(element => element.Name.LocalName == "ProjectReference")
                .Select(element => element.Attribute("Include")!.Value)
                .ToList();

            foreach (var reference in projectReferences)
            {
                string fullPath = Path.GetFullPath(Path.Combine(ProductionProjectDirectory, reference));

                Assert.Multiple(() =>
                {
                    Assert.That(reference, Does.Not.Contains("/src/"), $"Project reference '{reference}' uses a src/ layout, which is forbidden.");
                    Assert.That(Path.GetFileNameWithoutExtension(reference), Does.Not.Contain("Test"), $"'{reference}' references a test assembly; production code must not depend on tests.");
                    Assert.That(File.Exists(fullPath), Is.True, $"Referenced project '{reference}' resolves to '{fullPath}', which does not exist.");
                });
            }
        }

        [Test]
        public void GivenProductionProject_WhenProjectMetadataIsInspected_ThenRequiredPropertiesAreSet()
        {
            XDocument document = XDocument.Load(ProductionProjectFile);

            foreach (var (propertyName, expectedValue) in RequiredProjectProperties)
            {
                string actualValue = document
                    .Descendants()
                    .Where(element => element.Name.LocalName == propertyName)
                    .Select(element => element.Value.Trim())
                    .First();

                Assert.That(actualValue, Is.EqualTo(expectedValue), $"Property '{propertyName}' is '{actualValue}', expected '{expectedValue}'.");
            }
        }

        [Test]
        public void GivenSolutionFile_WhenProjectsAreListed_ThenMainProjectIsFirstFollowedByAlphabeticallyOrderedTestProjects()
        {
            XDocument solution = XDocument.Load(Path.Combine(RepositoryRoot, "NuciAPI.Middleware.Logging.slnx"));

            var projectPaths = solution
                .Root!.Elements()
                .Where(element => element.Name.LocalName == "Project")
                .Select(element => element.Attribute("Path")!.Value)
                .ToList();

            Assert.That(projectPaths.Count, Is.GreaterThan(0), "The solution declares no projects; ordering cannot be verified.");
            Assert.That(projectPaths[0], Is.EqualTo("NuciAPI.Middleware.Logging/NuciAPI.Middleware.Logging.csproj"), "The first project in the solution must be the main source project.");

            foreach (var path in projectPaths)
            {
                Assert.That(path, Does.Not.Contains("/src/"), $"Project '{path}' uses a src/ layout, which is forbidden.");
            }

            var testProjectNames = projectPaths
                .Skip(1)
                .Select(path => Path.GetFileNameWithoutExtension(path))
                .ToList();

            Assert.That(testProjectNames, Is.EqualTo(testProjectNames.OrderBy(name => name, System.StringComparer.Ordinal)), "Test projects are not ordered alphabetically by project name.");
        }

        [Test]
        public void GivenProductionTypes_WhenArchUnitNetRuleIsChecked_ThenExpectedTypesResideInTheProductionAssembly()
        {
            IArchRule rule = Types()
                .That()
                .ResideInNamespace("NuciAPI.Middleware.Logging")
                .Should()
                .ResideInAssemblyMatching("NuciAPI\\.Middleware\\.Logging");

            rule.Check(ProductionArchitecture);
        }
    }
}
