
using System.Collections.Generic;
namespace callcluster_dotnet.dto
{
    public class CallgraphDTO
    {
        public IEnumerable<CallDTO> calls {get; internal set;}
        public IEnumerable<FunctionDTO> functions {get; internal set;}
        public CommunityDTO community {get; internal set;}
    }
}