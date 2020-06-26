using System;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class CallgraphCollector : ICallCollector, IMethodCollector
    {
        private SymbolIndexer FunctionIndexer;

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

        public void AddCall(IMethodSymbol caller, ISymbol called)
        {
            long? callerIndex = FunctionIndexer.IndexOf(caller);
            long? calledIndex = FunctionIndexer.IndexOf(called);
            Console.WriteLine(callerIndex + " calls " + calledIndex);
        }
    }
}