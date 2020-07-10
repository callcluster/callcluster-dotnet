using System;
using System.Linq;
using System.Threading;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class CallgraphWalker
    {
        private CancellationToken CancellationToken;
        private SemanticModel CurrentModel;
        private CallgraphCollector Collector;

        public CallgraphWalker(){
            CancellationToken=new CancellationTokenSource().Token;
            CancellationToken.Register(()=>{
                Console.WriteLine("The action was cancelled");
            });
            this.Collector = new CallgraphCollector();
        }

        internal CallgraphDTO GetCallgraphDTO()
        {
            return this.Collector.GetCallgraphDTO();
        }

        internal void Visit(Solution solution)
        {
            Collector.SetCurrent(solution);
            foreach(ProjectId id in solution.GetProjectDependencyGraph().GetTopologicallySortedProjects().ToList()){
                Visit(solution.GetProject(id));
            }
        }

        internal void Visit(Project project)
        {
            Collector.SetCurrent(project);
            foreach(var document in project.Documents){
                Console.WriteLine(document.Name);
                var modelTask = document.GetSemanticModelAsync(CancellationToken);
                modelTask.Wait();
                this.CurrentModel = modelTask.Result;
                this.Collector.SetModel(CurrentModel);
                if(this.CurrentModel.Language=="C#"){
                    var walker = new CSharpCallgraphWalker(this.Collector,this.Collector, new CSharpMethodAnalyzer());
                    walker.Visit(this.CurrentModel);
                }
            }
        }
    }
}