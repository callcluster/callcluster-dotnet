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

        public override void VisitClassDeclaration(ClassDeclarationSyntax node)
        {
            INamedTypeSymbol symbol = CurrentModel.GetDeclaredSymbol(node);
            this.ClassCollector.AddClass(symbol);
            base.VisitClassDeclaration(node);
        }

        private void VisitBaseMethodDeclarationSyntax(BaseMethodDeclarationSyntax node)
        {
            this.CurrentMethod = CurrentModel.GetDeclaredSymbol(node);
            this.MethodCollector.AddMethod(this.CurrentMethod);
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            var previousMethod = this.CurrentMethod;
            var symbol = CurrentModel.GetDeclaredSymbol(node);
            var visitor = new InvocationExpressionSymbolVisitor();
            symbol.Accept(visitor);
            this.CurrentMethod = visitor.MethodSymbol;
            this.MethodCollector.AddMethod(this.CurrentMethod);
            base.VisitLocalFunctionStatement(node);
            this.CurrentMethod = previousMethod;
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            var previousMethod = this.CurrentMethod;
            VisitBaseMethodDeclarationSyntax(node);
            base.VisitMethodDeclaration(node);
            this.CurrentMethod = previousMethod;
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            var previousMethod = this.CurrentMethod;
            VisitBaseMethodDeclarationSyntax(node);
            if(node.Initializer!=null){
                ISymbol initializer = this.CurrentModel.GetSymbolInfo(node.Initializer).Symbol;
                InvocationExpressionSymbolVisitor visitor = new InvocationExpressionSymbolVisitor();
                initializer.Accept(visitor);
                this.CallCollector.AddCall(this.CurrentMethod,visitor.MethodSymbol,initializer.ContainingType);
            }
            base.VisitConstructorDeclaration(node);
            this.CurrentMethod = previousMethod;
        }

        public override void VisitInvocationExpression(InvocationExpressionSyntax node)
        {
            var visitor = new InvocationExpressionVisitor();
            node.Expression.Accept(visitor);
            TypeInfo calledType = CurrentModel.GetTypeInfo(node.Expression);
            IMethodSymbol called = visitor.GetCalledMethod(this.CurrentModel);

            this.MethodCollector.AddMethod(called);
            this.CallCollector.AddCall(this.CurrentMethod,called,visitor.GetCalledType(this.CurrentModel));
            base.VisitInvocationExpression(node);
        }

        public override void VisitObjectCreationExpression(ObjectCreationExpressionSyntax node)
        {
            var visitor = new InvocationExpressionSymbolVisitor();
            ISymbol symbol = this.CurrentModel.GetSymbolInfo(node).Symbol;
            symbol.Accept(visitor);
            this.MethodCollector.AddMethod(visitor.MethodSymbol);
            this.CallCollector.AddCall(this.CurrentMethod,visitor.MethodSymbol,visitor.MethodSymbol.ContainingType);
            base.VisitObjectCreationExpression(node);
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
    }

    internal class InvocationExpressionSymbolVisitor : SymbolVisitor
    {
        public IMethodSymbol MethodSymbol { get; private set; }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            this.MethodSymbol = symbol;
        }
    }
}
