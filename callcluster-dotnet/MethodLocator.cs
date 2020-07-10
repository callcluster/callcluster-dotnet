using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace callcluster_dotnet
{
    internal class MethodLocator
    {
        private Tree<ISymbol> tree;

        private class ParentalNameComprarer : IEqualityComparer<ISymbol>
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

        public MethodLocator()
        {
            this.tree = new Tree<ISymbol>(new ParentalNameComprarer());
        }

        internal void Add(IMethodSymbol method)
        {
            Add(method as ISymbol);
        }

        private void Add(ISymbol symbol)
        {
            if(symbol!=null && symbol.ContainingSymbol!=null)
            {
                tree.Add(symbol.ContainingSymbol,symbol);
                Add(symbol.ContainingSymbol as ISymbol);
            }
        }

        internal CommunityDTO GetCommunityDTO(SymbolIndexer functionIndexer)
        {
            return GetCommunityDTO(functionIndexer,tree.GetRoots());
        }

        private CommunityDTO GetCommunityDTO(SymbolIndexer functionIndexer, ICollection<ISymbol> collections)
        {
            return new CommunityDTO(){
                name="root",
                type="root",
                communities = collections.Select(s=>GetCommunityDTO(functionIndexer,s)),
                functions = new long[] {},
            };
        }

        private CommunityDTO GetCommunityDTO(SymbolIndexer functionIndexer, ISymbol s)
        {
            return new CommunityDTO(){
                name = s.Name,
                type = s.Kind.ToString(),
                communities = tree.ChildrenOf(s).Where(s=>s.Kind!=SymbolKind.Method).Select(s=>GetCommunityDTO(functionIndexer,s)),
                functions = tree.ChildrenOf(s).Where(s=>s.Kind==SymbolKind.Method).Select(s=>functionIndexer.IndexOf(s)).Where(x=>x.HasValue).Select(x=>x.Value)
            };
        }
    }
}