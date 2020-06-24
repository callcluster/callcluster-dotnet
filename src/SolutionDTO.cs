using System.Collections.Generic;

namespace callcluster_dotnet
{
    internal class SolutionDTO
    {
        public SolutionDTO()
        {
        }

        public string FilePath { get; internal set; }
        public IList<ProjectDTO> Projects { get; internal set; }
    }
}