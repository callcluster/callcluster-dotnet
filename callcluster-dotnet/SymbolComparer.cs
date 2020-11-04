using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class SymbolComparer : IEqualityComparer<ISymbol>
    {
        public bool Equals([AllowNull] ISymbol x, [AllowNull] ISymbol y)
        {
            return (x==null || y== null) || (
                x!=null && y!=null && x.Name==y.Name && x.Kind == y.Kind && (
                    (x.ContainingSymbol == null && y.ContainingSymbol == null)
                    ||
                    (x.ContainingSymbol!=null && y.ContainingSymbol!=null && Equals(x.ContainingSymbol,y.ContainingSymbol))
                )
            );
        }

        public int GetHashCode([DisallowNull] ISymbol obj)
        {
            int myHash = obj.Name.GetHashCode() ^ obj.Kind.GetHashCode();
            if(obj.ContainingSymbol==null)
            {
                return myHash;
            }
            else
            {
                return myHash ^ GetHashCode(obj.ContainingSymbol);
            }
        }
    }
}