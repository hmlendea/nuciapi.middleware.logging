using System;
using System.IO;
using System.Linq;
using System.Reflection;

using ArchUnitNET.Domain;
using ArchUnitNET.Loader;

namespace NuciAPI.Middleware.Logging.ArchitectureTests
{
    public abstract class ArchitectureTestsFixture
    {
        protected static string RepositoryRoot { get; } = FindRepositoryRoot();
        protected static string ProductionProjectDirectory { get; } = Path.Combine(RepositoryRoot, "NuciAPI.Middleware.Logging");
        protected static string ProductionProjectFile { get; } = Path.Combine(ProductionProjectDirectory, "NuciAPI.Middleware.Logging.csproj");
        protected static Architecture ProductionArchitecture { get; } = LoadProductionArchitecture();

        private static string FindRepositoryRoot()
        {
            DirectoryInfo? directory = Directory.GetParent(typeof(ArchitectureTestsFixture).Assembly.Location);
            while (directory != null && !Directory.EnumerateFiles(directory.FullName, "*.slnx").Any())
            {
                directory = directory.Parent;
            }

            if (directory == null)
            {
                throw new DirectoryNotFoundException("NuciAPI repository root (solution file) could not be located from the test assembly location.");
            }

            return directory.FullName;
        }

        private static Architecture LoadProductionArchitecture()
        {
            System.Reflection.Assembly productionAssembly = System.Reflection.Assembly.Load(new System.Reflection.AssemblyName("NuciAPI.Middleware.Logging"));
            return new ArchLoader().LoadAssemblies(productionAssembly).Build();
        }
    }
}
