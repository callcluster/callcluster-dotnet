using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    public interface IMethodCollector
    {
        void AddMethod(IMethodSymbol method);
        void AddMethod(ISymbol called);
    }
}