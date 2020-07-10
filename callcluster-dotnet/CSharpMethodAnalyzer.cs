
using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.FlowAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace callcluster_dotnet
{
    internal class CSharpMethodAnalyzer : ICSharpMethodAnalyzer
    {
        public MethodAnalysisData AnalyzeMethod(BaseMethodDeclarationSyntax syntax, SemanticModel model)
        {
            if(model.GetDeclaredSymbol(syntax).IsAbstract){
                return new MethodAnalysisData();
            }

            if(syntax.Body == null && syntax.ExpressionBody!=null){
                return new MethodAnalysisData(){
                    CyclomaticComplexity = 1,
                    NumberOfLines = 1,
                    NumberOfStatements = 1
                };
            }
            
            var analysis = AnalyzeSyntaxMethod(syntax.Body, model);
            var op = model.GetOperation(syntax);
            var visitor =  new ControlFlowGraphVisitor();
            op.Accept(visitor);

            analysis.CyclomaticComplexity = getCyclomaticComplexity(visitor.Graph);

            return analysis;
        }

        private MethodAnalysisData AnalyzeSyntaxMethod(BlockSyntax syntax, SemanticModel model)
        {

            var statementCounter = new StatementCounterWalker();
            syntax.Accept(statementCounter);
            return new MethodAnalysisData(){
                NumberOfLines = syntax.ToString().Split('\n').Count(),
                NumberOfStatements = statementCounter.NumberOfStatements,
            };
        }

        private int? getCyclomaticComplexity(ControlFlowGraph cfg)
        {
            if(cfg==null) return null;

            int outgoingEdges(BasicBlock  block)
            {
                int outgoing = 0;
                if(block.ConditionalSuccessor!=null){
                    outgoing+=1;
                }
                if(block.FallThroughSuccessor!=null){
                    outgoing+=1;
                }
                return outgoing;
            }

            int vertices = cfg.Blocks.Count();
            int edges = cfg.Blocks.Sum(outgoingEdges);

            return edges - vertices + 2;
        }

        public MethodAnalysisData AnalyzeMethod(LocalFunctionStatementSyntax syntax, SemanticModel model)
        {
            return AnalyzeSyntaxMethod(syntax.Body, model);
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

        private class ControlFlowGraphVisitor :  OperationVisitor
        {
            override public void VisitBlock(IBlockOperation operation)
            {
                Graph = ControlFlowGraph.Create(operation);
            }

            override public void VisitConstructorBodyOperation(IConstructorBodyOperation operation)
            {
                Graph = ControlFlowGraph.Create(operation);
            }

            override public void VisitFieldInitializer(IFieldInitializerOperation operation)
            {
                Graph = ControlFlowGraph.Create(operation);
            }

            override public void VisitMethodBodyOperation(IMethodBodyOperation operation)
            {
                Graph = ControlFlowGraph.Create(operation);
            }

            override public void VisitParameterInitializer(IParameterInitializerOperation operation)
            {
                Graph = ControlFlowGraph.Create(operation);
            }

            override public void VisitPropertyInitializer(IPropertyInitializerOperation operation)
            {
                Graph = ControlFlowGraph.Create(operation);
            }

            public ControlFlowGraph Graph { get; private set; }
        }
    }
}