using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using callcluster_dotnet.dto;

namespace callcluster_dotnet.Tests
{
    public class Utils
    {
        public static readonly string SOLUTIONS = "../../../../test-solutions/";
        internal static readonly IEqualityComparer<CallDTO> CallComparer;

        static Utils(){
            CallComparer= new CallComparer();
        }

        public static Task<CallgraphDTO> Extract(string file){
            return callcluster_dotnet.Program.Extract(Utils.SOLUTIONS+file);
        }

        public static long IndexOf(CallgraphDTO callgraph, string name){
            var i = callgraph.functions.ToList().FindIndex(x=>x.name==name);
            if(i<0)
            {
                throw new System.Exception($"{name} not present in functions.");
            }
            else
            {
                return i;
            }
        }

        public static FunctionDTO Named(CallgraphDTO callgraph, string name){
            var f = callgraph.functions.ToList().FirstOrDefault(x=>x.name==name);
            if(f==null)
            {
                throw new System.Exception($"{name} not present in functions.");
            }
            else
            {
                return f;
            }
        }

    }

    public static class CallgraphExtensions
    {
        public static CommunityDTO Child(this CommunityDTO com,string name)
        {
            return com.communities.FirstOrDefault(c=> c.name == name );
        }
    }

    internal class CallComparer : IEqualityComparer<CallDTO>
    {
        public bool Equals([AllowNull] CallDTO x, [AllowNull] CallDTO y)
        {
            return x.from.Equals(y.from) && x.to.Equals(y.to);
        }

        public int GetHashCode([DisallowNull] CallDTO obj)
        {
            return obj.from.GetHashCode() & obj.to.GetHashCode();
        }
    }
}