
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal class CSharpMethodAnalyzer : ICSharpMethodAnalyzer
    {
        public MethodAnalysisData AnalyzeMethod(BaseMethodDeclarationSyntax syntax)
        {

            var statementCounter = new StatementCounterWalker();
            syntax.Accept(statementCounter);

            var cyclomaticComplexityCalculator = new CyclomaticComplexityCalculatorWalker();
            syntax.Accept(cyclomaticComplexityCalculator);


            return new MethodAnalysisData(){
                NumberOfLines = syntax.ToString().Split('\n').Count(),
                NumberOfStatements = statementCounter.NumberOfStatements,
                CiclomaticComplexity = cyclomaticComplexityCalculator.Complexity,
            };
        }

        private class StatementCounterWalker : CSharpSyntaxWalker
        {
            public int NumberOfStatements { get; internal set; } = 0;
            public override void DefaultVisit(SyntaxNode node){
                if(node is StatementSyntax){
                    NumberOfStatements += 1;
                }
                base.DefaultVisit(node);
            }
            
        }

        private class CyclomaticComplexityCalculatorWalker : CSharpSyntaxWalker
        {
            public CyclomaticComplexityCalculatorWalker()
            {
            }

            public int Complexity { get; internal set; }
        }
    }
}