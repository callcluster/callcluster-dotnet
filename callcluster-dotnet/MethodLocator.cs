using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.FindSymbols;

namespace callcluster_dotnet
{
    internal partial class MethodLocator
    {
        private Tree<ISymbol> tree;

        public MethodLocator()
        {
            this.tree = new Tree<ISymbol>(SymbolEqualityComparer.Default);
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