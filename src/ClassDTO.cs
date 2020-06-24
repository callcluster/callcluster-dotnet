using System.Collections.Generic;
using System.Linq;

namespace callcluster_dotnet
{
    internal class ClassDTO
    {
        public string Name { get; internal set; }
        public string Accessibility { get; internal set; }
        public IEnumerable<SymbolDTO> ImplementedInterfaces { get; internal set; }
        public IEnumerable<MethodDTO> Methods { get; internal set; }
        public string Namespace { get; internal set; }
        public SymbolDTO ParentClass { get; internal set; }
        public string RealName { get; internal set; }

        public override string ToString()
        {
            string implementsText = string.Join(",", ImplementedInterfaces.Select(i => i.Name));
            string methodsText = string.Join("\n", Methods.Select(m => m.ToString()));
            return $"ClassDTO: {Name}, Accesibility: {Accessibility}, implements: {implementsText}, methods:\n" + methodsText;
        }
    }
}