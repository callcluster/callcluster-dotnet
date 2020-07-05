using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class Tree<T>
    {
        private IDictionary<T,ICollection<T>> ParentToChildren;
        private ICollection<T> Roots;
        private ICollection<T> NonRoots;
        public Tree()
        {
            this.ParentToChildren = new Dictionary<T,ICollection<T>>();
            this.Roots = new HashSet<T>();
            this.NonRoots = new HashSet<T>();
        }


        internal void Add(T parent, T child)
        {
            ICollection<T> children;
            this.ParentToChildren.TryGetValue(parent,out children);
            if(children==null){
                children = new HashSet<T>();
                this.ParentToChildren[parent] = children;
            }
            children.Add(child);

            if(Roots.Contains(child))
            {
                Roots.Remove(child);
                NonRoots.Add(child);
            }

            if(!NonRoots.Contains(parent))
            {
                Roots.Add(parent);
            }
        }

        internal ICollection<T> GetRoots()
        {
            return Roots;
        }

        internal ICollection<T> ChildrenOf(T x)
        {
            return ParentToChildren[x];
        }

        /// <summary>
        /// Descendants of a symbol. It doesn't check if the tree really is a tree.
        /// </summary>
        /// <param name="parent">root node from which to analyze</param>
        /// <returns>list of all descendants</returns>
        internal IEnumerable<T> DescendantsOf(T parent)
        {
            if(!ParentToChildren.ContainsKey(parent)){
                return new List<T>(){parent};
            }else{

                return ParentToChildren[parent].SelectMany((s)=>{
                    return DescendantsOf(s);
                }).Append(parent);
            }
        }
    }
}