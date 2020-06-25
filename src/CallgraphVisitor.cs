using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal class CallgraphVisitor : SymbolVisitor
    {
        private SemanticModel CurrentModel;

        internal object GetCallgraphDTO()
        {
            throw new NotImplementedException();
        }

        internal void Visit(Solution solution)
        {
            foreach(ProjectId id in solution.GetProjectDependencyGraph().GetTopologicallySortedProjects()){
                Visit(solution.GetProject(id));
            }
        }

        internal async void Visit(Project project)
        {
            foreach(var document in project.Documents){
                CurrentModel = await document.GetSemanticModelAsync();
                var syntaxRoot = await CurrentModel.SyntaxTree.GetRootAsync();
                CurrentModel.GetSymbolInfo(syntaxRoot).Symbol.Accept(this);
            }
        }
    }
}