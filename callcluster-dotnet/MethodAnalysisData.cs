using System;
using System.Collections.Generic;
using System.Linq;
using callcluster_dotnet.dto;
using Microsoft.CodeAnalysis;

namespace callcluster_dotnet
{
    public class MethodAnalysisData
    {
        public int? CyclomaticComplexity { get; internal set; }
        public int? NumberOfLines { get; internal set; }
        public int? NumberOfStatements { get; internal set; }
    }
}