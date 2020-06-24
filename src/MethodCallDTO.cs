using Microsoft.CodeAnalysis;
using System.Collections.Generic;

namespace callcluster_dotnet
{
    internal class MethodCallDTO
    {
        public string Name { get; internal set; }
        public string @class { get; internal set; }
        public string Assembly { get; internal set; }
        public string Module { get; internal set; }
        public IEnumerable<SymbolDTO> ParametersTypes { get; internal set; }
        public string RealName { get; internal set; }

        public override string ToString()
        {
            return $"MethodCallDTO: {Name}, From class: {@class}  Assembly: {Assembly}, Module: {Module}";
        }
    }
}