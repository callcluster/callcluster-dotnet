using System;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal class CSharpCallgraphWalker : CSharpSyntaxWalker
    {
        private SemanticModel CurrentModel;
        private NamespaceDeclarationSyntax CurrentNamespace;
        private MethodDeclarationSyntax CurrentMethod;

        public CSharpCallgraphWalker():base(SyntaxWalkerDepth.Node) { }

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
            this.CurrentMethod = node;
            base.VisitMethodDeclaration(node);
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node){
            Console.WriteLine(node.ToString()+" inside "+this.CurrentMethod.ToString());
            base.VisitInvocationExpression(node);
        }
    }
}