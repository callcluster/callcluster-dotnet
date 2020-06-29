using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    public interface ICallCollector
    {
        void AddCall(IMethodSymbol caller, IMethodSymbol called, ITypeSymbol calledType);
    }
}