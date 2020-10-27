
using System.Collections.Generic;
namespace callcluster_dotnet.dto
{
    public class FunctionDTO
    {
        public int? basiliComplexity { get; internal set; }
        public int? cyclomaticComplexity { get; internal set; }
        public int? linesOfCode { get; internal set; }
        public int? numberOfStatements { get; internal set; }
        public string location { get; internal set; }
        public string name {get; internal set;}
    }
}