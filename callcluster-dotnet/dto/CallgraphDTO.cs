
using System.Collections.Generic;
namespace callcluster_dotnet.dto
{
    internal class CallgraphDTO
    {
        public MetadataDTO metadata { get; internal set; }
        public IEnumerable<CallDTO> calls {get; internal set;}
        public IEnumerable<FunctionDTO> functions {get; internal set;}
        public CommunityDTO community {get; internal set;}
    }
}