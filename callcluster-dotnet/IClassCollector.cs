using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    public interface IClassCollector
    {
        void AddClass(INamedTypeSymbol symbol);
    }
}