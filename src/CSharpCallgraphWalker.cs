using System;
using System.Linq;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal class CSharpCallgraphWalker : CSharpSyntaxWalker
    {
        private SemanticModel CurrentModel;
        private NamespaceDeclarationSyntax CurrentNamespace;
        private IMethodSymbol CurrentMethod;
        private SymbolIndexer FunctionIndexer;

        public CSharpCallgraphWalker():base(SyntaxWalkerDepth.Node)
        {
            this.FunctionIndexer = new SymbolIndexer();
        }

        internal void Visit(SemanticModel model)
        {
            this.CurrentModel = model;
            Visit(this.CurrentModel.SyntaxTree.GetRoot());
        }

        public override void VisitNamespaceDeclaration(NamespaceDeclarationSyntax node)
        {
            this.CurrentNamespace = node;
            base.VisitNamespaceDeclaration(node);
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            this.CurrentMethod = CurrentModel.GetDeclaredSymbol(node);
            this.FunctionIndexer.Add(this.CurrentMethod);
            base.VisitMethodDeclaration(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node){
            var calledSymbol = CurrentModel.GetSymbolInfo(node.Expression).Symbol.OriginalDefinition;
            this.FunctionIndexer.Add(calledSymbol);
            long? calledIndex = this.FunctionIndexer.IndexOf(calledSymbol);
            long? callerIndex = this.FunctionIndexer.IndexOf(this.CurrentMethod);
            Console.WriteLine($"Function number {callerIndex} calls {calledIndex}");
            base.VisitInvocationExpression(node);
        }
    }
}
