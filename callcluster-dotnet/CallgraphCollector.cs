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
        private IList<(long? from, long? to)> Calls;

        public CallgraphCollector()
        {
            this.FunctionIndexer = new SymbolIndexer();
            this.Calls = new List<(long? from, long? to)>();
        }

        public void AddMethod(IMethodSymbol method)
        {
            FunctionIndexer.Add(method);
        }

        public void AddMethod(ISymbol called)
        {
            FunctionIndexer.Add(called);
        }

        public void AddCall(IMethodSymbol caller, ISymbol called, TypeInfo calledType)
        {
            long? callerIndex = FunctionIndexer.IndexOf(caller);
            long? calledIndex = FunctionIndexer.IndexOf(called);

            this.Calls.Add((from:callerIndex,to:calledIndex));
            Console.WriteLine(
                caller.ToString() + 
                callerIndex + 
                " calls " + 
                called.ToString() +
                calledIndex +
                " from type " +
                called.ContainingType +
                " from symbol " +
                called.ContainingSymbol +
                " the called type was " +
                calledType
            );
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
            return this.Calls.Where(c=>c.to.HasValue && c.from.HasValue).Select(c=>new CallDTO(){
                from=c.from.Value,
                to=c.to.Value
            });
        }

        internal void SetModel(SemanticModel currentModel)
        {
            this.CurrentModel = currentModel;
        }
    }
}