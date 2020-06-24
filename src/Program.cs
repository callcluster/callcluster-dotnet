using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.Build.Locator;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.CSharp;
using Newtonsoft.Json;
using System.IO;

namespace callcluster_dotnet
{
    class Program
    {
        static async Task Main(string[] args)
        {
            MSBuildLocator.RegisterDefaults();
            var _1 = typeof(Microsoft.CodeAnalysis.CSharp.Formatting.CSharpFormattingOptions);
            var _2 = typeof(Microsoft.CodeAnalysis.VisualBasic.VisualBasicDiagnosticFormatter);

            string solutionFilePath = args[0];

            IEnumerable<Project> projects = await ProjectsFromSolution(solutionFilePath);
            var json = JsonConvert.SerializeObject(await generateSolutionDTO(projects, solutionFilePath));
            File.WriteAllText(@"analysis.json", json);
        }

        private static async Task<SolutionDTO> generateSolutionDTO(IEnumerable<Project> projects, string solutionFilePath)
        {
            IList<ProjectDTO> projectsDTO = new List<ProjectDTO>();

            foreach(var project in projects)
            {
                projectsDTO.Add(await generateProjectDTO(project));
            }

            return new SolutionDTO()
            {
                FilePath = solutionFilePath,
                Projects = projectsDTO,
            };
        }

        private static ClassDTO generateClassDTO(ClassDeclarationSyntax @class, SemanticModel model)
        {
            var constructors = @class.DescendantNodes().OfType<ConstructorDeclarationSyntax>().OfType<BaseMethodDeclarationSyntax>();
            var methods = @class.DescendantNodes().OfType<MethodDeclarationSyntax>().OfType<BaseMethodDeclarationSyntax>();
            var declaredSymbol = model.GetDeclaredSymbol(@class);
            
            return new ClassDTO()
            {
                RealName = @class.Identifier.Text,
                Name = declaredSymbol.ToDisplayString(),
                Accessibility = declaredSymbol.DeclaredAccessibility.ToString(),
                Namespace=declaredSymbol.ContainingNamespace.Name,
                ImplementedInterfaces = declaredSymbol.AllInterfaces.Select(i => generateSymbolDTO(i)),
                ParentClass = declaredSymbol.BaseType == null?null: generateSymbolDTO(declaredSymbol.BaseType),
                Methods = constructors.Concat(methods).Select(m => generateMethodDTO(m, model)),
            };
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

        private static MethodDTO generateMethodDTO(BaseMethodDeclarationSyntax method, SemanticModel model)
        {
            var declaredSymbol = model.GetDeclaredSymbol(method);
            var invocations = method.DescendantNodes().OfType<InvocationExpressionSyntax>().OfType<ExpressionSyntax>();
            var creations = method.DescendantNodes().OfType<ObjectCreationExpressionSyntax>().OfType<ExpressionSyntax>();
            var calls = invocations.Concat(creations);
            return new MethodDTO()
            {
                //RealName = method.Identifier.Text,
                Name= declaredSymbol.ToDisplayString(),
                IsAbstract = declaredSymbol.IsAbstract,
                OverridenMethodName = declaredSymbol.OverriddenMethod?.ToDisplayString(),
                ParametersTypes = declaredSymbol.Parameters.Select(ps => generateSymbolDTO(ps)),
                Accessibility = declaredSymbol.DeclaredAccessibility.ToString(),
                LinesOfCode = method.ToString().Split('\n').Count(),
                Calls = calls.Select(i=>generateMethodCallDTO(i,model)),
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

        private static async Task<ProjectDTO> generateProjectDTO(Project project)
        {
            IList<ClassDTO> classes = new List<ClassDTO>();
            var documents = project.Documents.ToList();
            for (int i=0;i<documents.Count();i++)
            {
                var document = documents[i];
                Console.WriteLine($"Working on file {document.Name}, {i} of {documents.Count()}");
                var model = await document.GetSemanticModelAsync();
                var syntaxRoot = await model.SyntaxTree.GetRootAsync();
                var namespaces = syntaxRoot.ChildNodes().OfType<NamespaceDeclarationSyntax>();
                var classesFromThisDocument = namespaces.SelectMany(ns => ns.ChildNodes()).OfType<ClassDeclarationSyntax>();
                foreach (var @class in classesFromThisDocument)
                {
                    classes.Add(generateClassDTO(@class,model));
                }
            }

            return new ProjectDTO()
            {
                Name = project.Name,
                Assembly = project.AssemblyName,
                Classes = classes,
            };
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
