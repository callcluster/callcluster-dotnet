using System.Collections.Generic;

namespace callcluster_dotnet.old
{
    internal class ProjectDTO
    {
        public string Name { get; internal set; }
        public string Assembly { get; internal set; }
        public IEnumerable<ClassDTO> Classes { get; internal set; }
    }
}