
using System.Collections.Generic;
namespace callcluster_dotnet.dto
{
    public class CommunityDTO
    {
        public string name { get; internal set; }
        public string type { get; internal set; }
        
        public IEnumerable<long> functions {get; internal set;}
        public IEnumerable<CommunityDTO> communities {get; internal set;}
    }
}