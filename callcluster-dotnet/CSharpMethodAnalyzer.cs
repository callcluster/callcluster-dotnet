
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
                    NumberOfStatements = 1,
                    written = true
                };
            }
            
            var analysis = AnalyzeSyntaxMethod(syntax.Body, model);
            var op = model.GetOperation(syntax);
            if(op!=null){
                var visitor =  new ControlFlowGraphVisitor();
                op.Accept(visitor);
                analysis.CyclomaticComplexity = getCyclomaticComplexity(visitor.Graph);
            }

            return analysis;
        }

        private MethodAnalysisData AnalyzeSyntaxMethod(BlockSyntax syntax, SemanticModel model)
        {
            if(syntax==null){
                return new MethodAnalysisData(){
                    NumberOfLines = 1,
                    NumberOfStatements = 1,
                    BasiliComplexity = 1,
                    written=true
                };
            }else{
                var statementCounter = new StatementCounterWalker();
                syntax.Accept(statementCounter);
                var basiliWalker = new BasiliWalker();
                syntax.Accept(basiliWalker);
                return new MethodAnalysisData(){
                    NumberOfLines = syntax.ToString().Split('\n').Count(),
                    NumberOfStatements = statementCounter.NumberOfStatements,
                    BasiliComplexity = basiliWalker.Complexity,
                    written = true
                };
            }
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

        private class BasiliWalker : CSharpSyntaxWalker
        {
            public int Complexity { get; internal set; } = 1;
            
            override public void VisitIfStatement(IfStatementSyntax node){
                Complexity++;
                base.VisitIfStatement(node);
            }

            override public void VisitWhileStatement(WhileStatementSyntax node){
                Complexity++;
                base.VisitWhileStatement(node);
            }

            override public void VisitForStatement(ForStatementSyntax node){
                Complexity++;
                base.VisitForStatement(node);
            }

            override public void VisitForEachStatement(ForEachStatementSyntax node){
                Complexity++;
                base.VisitForEachStatement(node);
            }

            override public void VisitForEachVariableStatement(ForEachVariableStatementSyntax node){
                Complexity++;
                base.VisitForEachVariableStatement(node);
            }

            override public void VisitDoStatement(DoStatementSyntax node){
                Complexity++;
                base.VisitDoStatement(node);
            }

            override public void VisitCasePatternSwitchLabel (CasePatternSwitchLabelSyntax node){
                Complexity++;
                base.VisitCasePatternSwitchLabel(node);
            }

            override public void VisitCaseSwitchLabel(CaseSwitchLabelSyntax node){
                Complexity++;
                base.VisitCaseSwitchLabel(node);
            }

            override public void VisitBinaryExpression(BinaryExpressionSyntax node){
                switch(node.OperatorToken.Kind()){
                    case SyntaxKind.AmpersandAmpersandToken: Complexity++;
                    break;
                    case SyntaxKind.BarBarToken: Complexity++;
                    break;
                }
                base.VisitBinaryExpression(node);
            }

        }
    }
}