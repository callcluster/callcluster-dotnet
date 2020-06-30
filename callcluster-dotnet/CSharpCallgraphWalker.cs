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
        private IClassCollector ClassCollector;

        public CSharpCallgraphWalker(ICallCollector callCollector, IMethodCollector methodCollector, IClassCollector classCollector):base(SyntaxWalkerDepth.Node)
        {
            this.CallCollector = callCollector;
            this.MethodCollector = methodCollector;
            this.ClassCollector = classCollector;
        }

        internal void Visit(SemanticModel model)
        {
            this.CurrentModel = model;
            Visit(this.CurrentModel.SyntaxTree.GetRoot());
        }

        public override void VisitClassDeclaration(ClassDeclarationSyntax node){
            INamedTypeSymbol symbol = CurrentModel.GetDeclaredSymbol(node);
            this.ClassCollector.AddClass(symbol);
            base.VisitClassDeclaration(node);
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
            TypeInfo calledType = CurrentModel.GetTypeInfo(node.Expression);//CurrentModel.GetTypeInfo((node.Expression as MemberAccessExpressionSyntax).Expression);
            IMethodSymbol called = visitor.GetCalledMethod(this.CurrentModel);//CurrentModel.GetSymbolInfo(node.Expression).Symbol;

            this.MethodCollector.AddMethod(called);
            this.CallCollector.AddCall(this.CurrentMethod,called,visitor.GetCalledType(this.CurrentModel));
        }
    }

    internal class InvocationExpressionVisitor : CSharpSyntaxVisitor
    {
        private SyntaxNode VisitedNode;
        private MemberAccessExpressionSyntax MemberAccessNode;
        private IdentifierNameSyntax IndentifierNameNode;

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

        internal IMethodSymbol GetCalledMethod(SemanticModel model)
        {
            ExpressionSyntax nonNullNode;
            if(MemberAccessNode != null){
                nonNullNode=MemberAccessNode;
            } else if (IndentifierNameNode != null){
                nonNullNode=IndentifierNameNode;
            }else{
                return null;
            }
            InvocationExpressionSymbolVisitor visitor = new InvocationExpressionSymbolVisitor();
            model.GetSymbolInfo(nonNullNode).Symbol.Accept(visitor);
            return visitor.MethodSymbol;
        }

        private class InvocationExpressionSymbolVisitor : SymbolVisitor
        {
            public IMethodSymbol MethodSymbol { get; private set; }

            public override void VisitMethod(IMethodSymbol symbol)
            {
                this.MethodSymbol = symbol;
            }
        }
    }
}
