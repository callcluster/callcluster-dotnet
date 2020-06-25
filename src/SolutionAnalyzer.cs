using System;
using System.Linq;
using System.Threading.Tasks;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.MSBuild;

namespace callcluster_dotnet
{
    internal class SolutionAnalyzer
    {
        private Solution solution;
        public SolutionAnalyzer(Solution solution)
        {
            this.solution = solution;
        }

        internal async Task<CallgraphDTO> GetCallgraph()
        {
            
            
        }
    }
}