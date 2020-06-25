using System;
using System.Linq;
using System.Threading;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal class CallgraphVisitor : SymbolVisitor
    {
        private CancellationToken CancellationToken;

        public CallgraphVisitor(){
            CancellationToken=new CancellationTokenSource().Token;
            CancellationToken.Register(()=>{
                Console.WriteLine("The action was cancelled");
            });
        }

        internal CallgraphDTO GetCallgraphDTO()
        {
            return new CallgraphDTO();
        }

        internal void Visit(Solution solution)
        {
            foreach(ProjectId id in solution.GetProjectDependencyGraph().GetTopologicallySortedProjects().ToList()){
                Visit(solution.GetProject(id));
            }
        }

        internal async void Visit(Project project)
        {
            foreach(var document in project.Documents){
                var modelTask = document.GetSemanticModelAsync(CancellationToken);
                modelTask.Wait();
                var model = modelTask.Result;
                var syntaxRoot = model.SyntaxTree.GetRoot();
                var rootInfo = model.GetSymbolInfo(syntaxRoot);
                rootInfo.Symbol.Accept(this);
            }
        }

        public override void DefaultVisit(ISymbol symbol)
        {
            Console.WriteLine(symbol.ToDisplayString());
        }
    }
}