
using System.Collections.Generic;
namespace callcluster_dotnet.dto
{
    internal class CommunityDTO
    {
        public string name { get; internal set; }
        public IEnumerable<long> functions {get; internal set;}
        public IEnumerable<CommunityDTO> communities {get; internal set;}
    }
}