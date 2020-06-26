using System.Threading.Tasks;
using callcluster_dotnet.dto;

namespace callcluster_dotnet.Tests
{
    public class Utils
    {
        public static readonly string SOLUTIONS = "../../../../test-solutions/";
        public static Task<CallgraphDTO> Extract(string file){
            return callcluster_dotnet.Program.Extract(Utils.SOLUTIONS+file);
        }
    }
}