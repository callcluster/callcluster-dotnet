
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using callcluster_dotnet.dto;
using Microsoft.Build.Locator;
using System;
using Newtonsoft.Json;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;
using System.Runtime.Loader;

namespace callcluster_dotnet
{
    public class Program
    {
        static async Task Main(string[] args)
        {
            var extracted = await Extract(args[0]);
            var json = JsonConvert.SerializeObject(extracted);
            File.WriteAllText(@"analysis.json", json);
        }

        public static async Task<CallgraphDTO> Extract(string filePath)
        {
            if (!MSBuildLocator.IsRegistered && MSBuildLocator.CanRegister)
            {
                var instance = MSBuildLocator.RegisterDefaults();
                AssemblyLoadContext.Default.Resolving += (assemblyLoadContext, assemblyName) =>
                {
                    var path = Path.Combine(instance.MSBuildPath, assemblyName.Name + ".dll");
                    if (File.Exists(path))
                    {
                        return assemblyLoadContext.LoadFromAssemblyPath(path);
                    }else{
                        Console.WriteLine("Callcluster had problems loading an assembly, some references will not be loaded correctly: "+assemblyName);
                        Console.WriteLine("Attempted to load it from "+path);
                        Console.WriteLine("Please install "+assemblyName+" at "+ path);
                    }

                    return null;
                };

            }

            var _1 = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);
            var _2 = typeof(Microsoft.CodeAnalysis.VisualBasic.VisualBasicDiagnosticFormatter);


            CallgraphWalker walker = new CallgraphWalker();
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (sender, workspaceFailedArgs) =>
                {
                    if (workspaceFailedArgs.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                    {
                        Console.WriteLine("Error when opening a file");
                        Console.WriteLine(workspaceFailedArgs.Diagnostic.Message);
                    }
                };
                if (Regex.Match(filePath, @".*\.sln$").Success)
                {
                    walker.Visit(await workspace.OpenSolutionAsync(filePath));
                }
                else if (Regex.Match(filePath, @".*\.csproj$").Success)
                {
                    walker.Visit(await workspace.OpenProjectAsync(filePath));
                }
                else
                {
                    Console.WriteLine("Not a solution or a project file.");
                    return null;
                }
            }

            return walker.GetCallgraphDTO();
        }
    }
}
