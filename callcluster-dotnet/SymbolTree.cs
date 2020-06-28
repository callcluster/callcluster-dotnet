using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class SymbolTree
    {
        private IDictionary<ISymbol,ICollection<ISymbol>> ParentToChildren;
        private IDictionary<ISymbol,ISymbol> ChildToParent;

        public SymbolTree()
        {
            this.ChildToParent = new Dictionary<ISymbol,ISymbol>();
            this.ParentToChildren = new Dictionary<ISymbol,ICollection<ISymbol>>();
        }
        internal void Add(ISymbol parent, ISymbol child)
        {
            this.ChildToParent.Add(child,parent);
            ICollection<ISymbol> children;
            this.ParentToChildren.TryGetValue(parent,out children);
            if(children==null){
                children = new HashSet<ISymbol>();
                this.ParentToChildren[parent] = children;
            }
            children.Add(child);
        }

    }
}