using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal class CSharpCallgraphWalker : CSharpSyntaxWalker
    {
        private SemanticModel CurrentModel;
        private IMethodSymbol CurrentMethod;
        private ICallCollector CallCollector;
        private IMethodCollector MethodCollector;

        public CSharpCallgraphWalker(ICallCollector callCollector, IMethodCollector methodCollector):base(SyntaxWalkerDepth.Node)
        {
            this.CallCollector = callCollector;
            this.MethodCollector = methodCollector;
        }

        internal void Visit(SemanticModel model)
        {
            this.CurrentModel = model;
            Visit(this.CurrentModel.SyntaxTree.GetRoot());
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            this.CurrentMethod = CurrentModel.GetDeclaredSymbol(node);
            this.MethodCollector.AddMethod(this.CurrentMethod);
            base.VisitMethodDeclaration(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node){
            var calledSymbol = CurrentModel.GetSymbolInfo(node.Expression).Symbol.OriginalDefinition;
            this.MethodCollector.AddMethod(calledSymbol);
            this.CallCollector.AddCall(this.CurrentMethod,calledSymbol);
            base.VisitInvocationExpression(node);
        }
    }
}
