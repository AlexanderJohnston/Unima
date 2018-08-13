using System.Collections.Generic;
using System.Linq;
using Cama.Core.Mutation.Analyzer;
using Cama.Core.Mutation.Mutators;
using Cama.Core.Mutation.Mutators.BinaryExpressionMutators;
using Cama.Core.Services;
using Cama.Core.Services.Project;
using Cama.Core.TestRunner;

namespace Cama.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            Do();

            System.Console.WriteLine("Waiting...");
            System.Console.ReadLine();
        }

        private static async void Do()
        {
            var projectLoader = new ProjectService();
            var config = projectLoader.OpenProject(@"C:\Users\Milleb\Documents\Cama\Projects\TesturaCode\TesturaCode.cama");

            var someService = new SomeService(new UnitTestAnalyzer());
            var files = await someService.DoSomeWorkAsync(config, new List<IMutator> { new MathMutator() });

            var testRunner = new TestRunnerService(new MutatedDocumentCompiler(), new DependencyFilesHandler(), new TestRunner());
            var result = await testRunner.RunTestAsync(files.Where(f => f.StatementsMutations.Any()).ToList()[1].StatementsMutations.FirstOrDefault(), @"C:\Users\Milleb\Documents\Cama\Testura.Code-master\src\Testura.Code.Tests\bin\Debug");

        }
    }
}