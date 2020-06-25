
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using callcluster_dotnet.dto;
using Microsoft.Build.Locator;
using System;
using Newtonsoft.Json;
using System.IO;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            var _1 = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);
            var _2 = typeof(Microsoft.CodeAnalysis.VisualBasic.VisualBasicDiagnosticFormatter);

            string filePath = args[0];

            CallgraphVisitor visitor = new CallgraphVisitor();
            using (var workspace = MSBuildWorkspace.Create())
            {
                workspace.WorkspaceFailed += (sender, workspaceFailedArgs) =>
                {
                    if (workspaceFailedArgs.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                    {
                        Console.WriteLine(workspaceFailedArgs.Diagnostic.Message);
                    }
                };
                if(Regex.Match(filePath,@".*\.sln$").Success){
                    visitor.Visit(await workspace.OpenSolutionAsync(filePath));
                }else if(Regex.Match(filePath,@".*\.csproj$").Success){
                    visitor.Visit(await workspace.OpenProjectAsync(filePath));
                }else{
                    Console.WriteLine("Not a solution or a project file.");
                    return;
                }
            }

            var json = JsonConvert.SerializeObject(visitor.GetCallgraphDTO());
            File.WriteAllText(@"analysis.json", json);
        }
    }
}
