
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

            

            //IEnumerable<Project> projects = await ProjectsFromSolution(solutionFilePath);
            var json = JsonConvert.SerializeObject(visitor.GetCallgraphDTO());
            File.WriteAllText(@"analysis.json", json);
        }


        

        public static SymbolDTO generateSymbolDTO(ISymbol @interface)
        {
            return new SymbolDTO()
            {
                Name = @interface.ToDisplayString(),
                RealName = @interface.Name,
                Assembly = @interface.ContainingAssembly?.Name,
                Module = @interface.ContainingModule?.Name
            };
        }

        

        private static MethodCallDTO generateMethodCallDTO(ExpressionSyntax invocation, SemanticModel model)
        {
            var symbol = model.GetSymbolInfo(invocation).Symbol;
            var argumentTypes = invocation.DescendantNodes().OfType<ArgumentListSyntax>().FirstOrDefault()?.Arguments.Select(a=>model.GetTypeInfo(a.Expression).ConvertedType);

            if (symbol == null)
            {
                return null;
            }
            else
            {
                return new MethodCallDTO()
                {
                    RealName = symbol.Name,
                    Name = symbol.ToDisplayString(),
                    ParametersTypes = argumentTypes?.Select(ps => generateSymbolDTO(ps)) ?? new List<SymbolDTO>(),
                    Assembly = symbol.ContainingAssembly?.Name,
                    Module = symbol.ContainingModule?.Name,
                    @class = symbol.ContainingType?.ToDisplayString(),
                };
            }
        }

        

        private static async Task<SemanticModel> getSemanticModel(string filename, Project project)
        {
            var analyzedFile = project.Documents.First(d => d.Name.Equals(filename));
            return await analyzedFile.GetSemanticModelAsync();
        }

        private static async Task<ClassDeclarationSyntax> getClassSyntax(string className, SemanticModel model)
        {
            var syntaxTreeRoot = (await model.SyntaxTree.GetRootAsync());
            var namespaceSyntaxes = syntaxTreeRoot.ChildNodes().OfType<NamespaceDeclarationSyntax>();
            var classes = namespaceSyntaxes.SelectMany(ns => ns.ChildNodes()).OfType<ClassDeclarationSyntax>();
            return classes.First(c => c.Identifier.Text == className);
        }

        private static async Task<IEnumerable<Project>> ProjectsFromSolution(string solutionFile)
        {
            using (var workspace = MSBuildWorkspace.Create())
            {
                Project GetProject(ProjectId id)
                {
                    return workspace.CurrentSolution.GetProject(id);
                }

                workspace.WorkspaceFailed += (sender, workspaceFailedArgs) =>
                {
                    if (workspaceFailedArgs.Diagnostic.Kind == WorkspaceDiagnosticKind.Failure)
                    {
                        Console.WriteLine(workspaceFailedArgs.Diagnostic.Message);
                    }
                };

                var solution = await workspace.OpenSolutionAsync(solutionFile);
                var projects = workspace.CurrentSolution.GetProjectDependencyGraph();

                var actualProjects = projects.GetTopologicallySortedProjects().Select(GetProject);

                var ret = actualProjects.ToList();

                return ret;
            }
        }

    }
}
