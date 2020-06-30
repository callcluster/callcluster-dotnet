using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class CallgraphCollector : ICallCollector, IMethodCollector, IClassCollector
    {
        private SymbolIndexer FunctionIndexer;
        private SemanticModel CurrentModel;
        private IList<(IMethodSymbol from, IMethodSymbol to, ITypeSymbol type)> Calls;

        /// <summary>
        /// A tree of overriden methods and how the methods override each other. The parent method is overriden by the child.
        /// </summary>
        private Tree<IMethodSymbol> MethodTree;
        /// <summary>
        /// A tree of inherited classes.
        /// </summary>
        private Tree<ITypeSymbol> ClassTree;

        public CallgraphCollector()
        {
            this.FunctionIndexer = new SymbolIndexer();
            this.Calls = new List<(IMethodSymbol from, IMethodSymbol to, ITypeSymbol type)>();
            this.MethodTree = new Tree<IMethodSymbol>();
            this.ClassTree = new Tree<ITypeSymbol>();
        }

        public void AddMethod(IMethodSymbol method)
        {
            if(method.OverriddenMethod != null)
            {
                FunctionIndexer.Add(method.OverriddenMethod);
                MethodTree.Add(method.OverriddenMethod, method);
            }
            FunctionIndexer.Add(method);
        }

        public void AddMethod(ISymbol called)
        {
            FunctionIndexer.Add(called);
        }

        public void AddCall(IMethodSymbol caller, IMethodSymbol called, ITypeSymbol calledType)
        {
            long? callerIndex = FunctionIndexer.IndexOf(caller);
            long? calledIndex = FunctionIndexer.IndexOf(called);

            this.Calls.Add((from:caller,to:called,type:calledType));
        }

        internal CallgraphDTO GetCallgraphDTO()
        {
            return new CallgraphDTO(){
                metadata= new MetadataDTO(){
                    language="C#"
                },
                functions = FunctionIndexer.GetFunctionDTOs(),
                calls = GetCallDTOs(),
                community = new CommunityDTO()
            };
        }

        private IEnumerable<CallDTO> GetCallDTOs()
        {

            //TODO: this.Calls should have symbols, and this method should be superb complex, handling inheritance
            //hay que poner la intersección de: 
            //- los métodos hijos del calledmethod con (implementado)
            //- los métodos hijos del calledmethod que están en clases hijas del tipo del punto de la llamada (not yet)
            //NECESITO LA JERARQUÍA DE CLASES PARA ESO!!!!
            return this.Calls.SelectMany((call)=>{
                long? from = FunctionIndexer.IndexOf(call.from);
                long? to = FunctionIndexer.IndexOf(call.to);

                if(!(from.HasValue && to.HasValue))
                {
                    return new List<CallDTO>();
                }
                else
                {
                    IEnumerable<IMethodSymbol> targetMethods = this.MethodTree.DescendantsOf(call.to);
                    IEnumerable<ITypeSymbol> descendantClasses = this.ClassTree.DescendantsOf(call.type);
                    IEnumerable<IMethodSymbol> filteredMethods = targetMethods.Where(s=>descendantClasses.Contains(s.ContainingType));

                    if(!filteredMethods.Contains(call.to)){
                        filteredMethods = filteredMethods.Append(call.to);
                    }

                    return filteredMethods.Select(s=>{
                        return new CallDTO(){
                            from = from.Value,
                            to = FunctionIndexer.IndexOf(s).Value
                        };
                    });

                }
            });
        }
        internal void SetModel(SemanticModel currentModel)
        {
            this.CurrentModel = currentModel;
        }

        public void AddClass(INamedTypeSymbol symbol)
        {
            if(symbol.BaseType != null)
            {
                ClassTree.Add(symbol.BaseType,symbol);
                if(symbol.BaseType != symbol){//object inherits from object
                    AddClass(symbol.BaseType);
                }
            }
        }
    }
}