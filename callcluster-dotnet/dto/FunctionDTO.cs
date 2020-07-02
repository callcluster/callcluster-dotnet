
using System.Collections.Generic;
namespace callcluster_dotnet.dto
{
    public class FunctionDTO
    {
        public int? cyclomaticComplexity { get; internal set; }
        public int? numberOfLines { get; internal set; }
        public int? numberOfStatements { get; internal set; }
        public string location { get; internal set; }
        public string name {get; internal set;}
    }
}