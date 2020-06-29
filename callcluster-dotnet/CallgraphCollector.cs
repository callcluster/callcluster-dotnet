using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class CallgraphCollector : ICallCollector, IMethodCollector
    {
        private SymbolIndexer FunctionIndexer;
        private SemanticModel CurrentModel;
        private IList<(ISymbol from, ISymbol to)> Calls;
        private SymbolTree MethodTree;

        public CallgraphCollector()
        {
            this.FunctionIndexer = new SymbolIndexer();
            this.Calls = new List<(ISymbol from, ISymbol to)>();
            this.MethodTree = new SymbolTree();
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

        public void AddCall(IMethodSymbol caller, ISymbol called, ITypeSymbol calledType)
        {
            long? callerIndex = FunctionIndexer.IndexOf(caller);
            long? calledIndex = FunctionIndexer.IndexOf(called);

            this.Calls.Add((from:caller,to:called));
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
                    return this.MethodTree.DescendantsOf(call.to).Select(s=>{
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
    }
}