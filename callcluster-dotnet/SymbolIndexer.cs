using System;
using System.Collections.Generic;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    internal class SymbolIndexer
    {
        private Dictionary<ISymbol, long?> IndexesDict;
        private int LastIndex;

        public SymbolIndexer()
        {
            this.IndexesDict = new Dictionary<ISymbol,long?>();
            this.LastIndex = 0;
        }

        internal void Add(ISymbol calledSymbol)
        {
            if(!this.IndexesDict.GetValueOrDefault(calledSymbol).HasValue){
                this.IndexesDict.Add(calledSymbol,this.LastIndex);
                this.LastIndex += 1;
            }
        }

        internal long? IndexOf(ISymbol symbol)
        {
            return this.IndexesDict.GetValueOrDefault(symbol);
        }
    }
}