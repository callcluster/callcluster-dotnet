using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
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
        /// <summary>
        /// Descendants of a symbol. It doesn't check if the tree really is a tree.
        /// </summary>
        /// <param name="parent">root node from which to analyze</param>
        /// <returns>list of all descendants</returns>
        internal IEnumerable<ISymbol> DescendantsOf(ISymbol parent)
        {
            if(!ParentToChildren.ContainsKey(parent)){
                return new List<ISymbol>(){parent};
            }else{

                return ParentToChildren[parent].SelectMany((s)=>{
                    return DescendantsOf(s);
                }).Append(parent);
            }
        }
    }
}