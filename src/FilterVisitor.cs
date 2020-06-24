using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class FilterVisitor : SymbolVisitor
    {
        private string NameFilter;
        public virtual INamedTypeSymbol Classes { get; private set; }

        public FilterVisitor(string nameFilter)
        {
            this.NameFilter = nameFilter;
        }

        public override void VisitNamedType(INamedTypeSymbol symbol)
        {
            //symbol.TypeKind.
        }
    }
}