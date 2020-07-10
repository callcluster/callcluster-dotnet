using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    public interface IMethodCollector
    {
        void AddMethod(IMethodSymbol method, MethodAnalysisData analysisData, IOperation operation);
        void AddMethod(ISymbol called);
        void AddMethod(IMethodSymbol method);
    }
}