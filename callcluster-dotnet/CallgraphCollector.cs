using System;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class CallgraphCollector : ICallCollector, IMethodCollector
    {
        private SymbolIndexer FunctionIndexer;
        private SemanticModel CurrentModel;

        public CallgraphCollector()
        {
            this.FunctionIndexer = new SymbolIndexer();
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

        internal void SetModel(SemanticModel currentModel)
        {
            this.CurrentModel = currentModel;
        }
    }
}