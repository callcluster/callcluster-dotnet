using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    public static class SymbolExtensions
    {
        public static bool IsInterfaceMember(this ISymbol symbol)
        {
            return (
                symbol.Kind == SymbolKind.Method && 
                symbol.ContainingType.BaseType == null
            );
        }

        public static bool IsInterface(this ISymbol symbol)
        {
            return (
                symbol.Kind == SymbolKind.NamedType && 
                (symbol as INamedTypeSymbol).BaseType == null
            );
        }
    }
}