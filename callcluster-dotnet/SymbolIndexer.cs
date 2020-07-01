using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
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

        internal IEnumerable<FunctionDTO> GetFunctionDTOs()
        {

            string LocationAsString(Location location)
            {
                if(location.IsInMetadata)
                {
                    return "module: "+location.MetadataModule.ToString();
                }
                else
                {
                    var span = location.GetMappedLineSpan();
                    return "file: "+span.Path+" line: "+ span.StartLinePosition.Line;
                }
            }
            return IndexesDict.Keys.OrderBy(s=>IndexesDict[s]).Select(s => new FunctionDTO(){
                location = LocationAsString(s.OriginalDefinition.Locations.FirstOrDefault()),
                name = s.ToDisplayString()
            });
        }
    }
}