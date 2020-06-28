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
            //hay que poner un sub-walker acá!! no todas las invocationExpressionSyntax tienen un MemberAccessExpressionSyntax!!
            var visitor = new InvocationExpressionVisitor();
            node.Expression.Accept(visitor);
            ISymbol called = CurrentModel.GetSymbolInfo(node.Expression).Symbol;
            TypeInfo calledType = CurrentModel.GetTypeInfo(node.Expression);//CurrentModel.GetTypeInfo((node.Expression as MemberAccessExpressionSyntax).Expression);

            this.MethodCollector.AddMethod(called);
            this.CallCollector.AddCall(this.CurrentMethod,called,visitor.GetCalledType(this.CurrentModel));
        }
    }

    internal class InvocationExpressionVisitor : CSharpSyntaxVisitor
    {
        private SyntaxNode VisitedNode;

        private MemberAccessExpressionSyntax MemberAccessNode;
        private IdentifierNameSyntax IndentifierNameNode;

        public InvocationExpressionVisitor()
        {
        }

        override public void DefaultVisit(SyntaxNode node){
            this.VisitedNode = node;
        }

        override public void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            MemberAccessNode=node;
        }

        override public void VisitIdentifierName(IdentifierNameSyntax node){
            IndentifierNameNode =node;
        }

        internal ITypeSymbol GetCalledType(SemanticModel model)
        {
            if(MemberAccessNode != null){
                return model.GetTypeInfo(MemberAccessNode.Expression).Type;
            } else if (IndentifierNameNode != null){
                return model.GetSymbolInfo(IndentifierNameNode).Symbol.ContainingType;
            }else{
                return null;
            }
        }
    }
}
