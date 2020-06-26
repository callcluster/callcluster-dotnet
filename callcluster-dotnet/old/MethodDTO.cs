using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis.Text;

namespace callcluster_dotnet.old
{
    internal class MethodDTO
    {
        public string Name { get; internal set; }
        public string Accessibility { get; internal set; }
        public int LinesOfCode { get; internal set; }
        public IEnumerable<MethodCallDTO> Calls { get; internal set; }
        public IEnumerable<SymbolDTO> ParametersTypes { get; internal set; }
        public string RealName { get; internal set; }
        public bool IsAbstract { get; internal set; }
        public string OverridenMethodName { get; internal set; }

        public override string ToString()
        {
            string callsText = string.Join("\n", Calls.Where(c=>c!=null).Select(c => c.ToString()));
            return $"MethodDTO: {Name} Accesibility: {Accessibility} LOC:{LinesOfCode}, Calls:\n" + callsText;
        }
    }
}