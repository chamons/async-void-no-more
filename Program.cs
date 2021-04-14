using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Mono.Options;

namespace async_void_no_more
{
    class Program
    {
        static async Task<int> Main(string[] args)
        {
            bool showHelp = false;
            var p = new OptionSet () {
				{ "h|help",  "show this message and exit", v => showHelp = true },
			};

			List<string> files = null;
			try {
				files = p.Parse (args);
			}
			catch (OptionException) {
				showHelp = true;
			}
            if (showHelp || files.Count != 1)
            {
                Console.WriteLine ("Usage: async-void-no-more.exe [OPTIONS]+ PROJECT.{csproj,sln}");
                Console.WriteLine ("Find async void test methods that should return Task.");
                Console.WriteLine ();
                Console.WriteLine ("Options:");
                p.WriteOptionDescriptions (Console.Out);
                return -1;
            }

            MSBuildLocator.RegisterDefaults();
            await Search (files[0]);
 
            return 0;
        }

        static async Task Search (string path)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                var project = await workspace.OpenProjectAsync(path);
                var compilation = await project.GetCompilationAsync();

                foreach (var tree in compilation.SyntaxTrees)
                {
                    var root = (CompilationUnitSyntax)tree.GetRoot ();
                    foreach (var n in root.DescendantNodes ().OfType<NamespaceDeclarationSyntax>())
                    {
                        if (n.Name.ToString().Contains(".Tests"))
                        {
                            foreach (var c in root.DescendantNodes ().OfType<ClassDeclarationSyntax> ())
                            {
                                foreach (var m in c.DescendantNodes().OfType<MethodDeclarationSyntax>())
                                {
                                    bool hasAwait = m.DescendantNodes ().OfType<AwaitExpressionSyntax>().Any();
                                    if (hasAwait && !m.ReturnType.ToString().Contains("Task"))
                                    {
                                        Console.WriteLine (m.Identifier);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
