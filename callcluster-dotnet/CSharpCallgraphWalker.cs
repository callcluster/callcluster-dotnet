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
        private ICSharpMethodAnalyzer MethodAnalyzer;

        public CSharpCallgraphWalker(ICallCollector callCollector, IMethodCollector methodCollector, IClassCollector classCollector, ICSharpMethodAnalyzer methodAnalyzer):base(SyntaxWalkerDepth.Node)
        {
            this.CallCollector = callCollector;
            this.MethodCollector = methodCollector;
            this.ClassCollector = classCollector;
            this.MethodAnalyzer = methodAnalyzer;
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
            this.MethodCollector.AddMethod(this.CurrentMethod, this.MethodAnalyzer.AnalyzeMethod(node));
        }

        public override void VisitLocalFunctionStatement(LocalFunctionStatementSyntax node)
        {
            var previousMethod = this.CurrentMethod;
            var symbol = CurrentModel.GetDeclaredSymbol(node);
            var visitor = new InvocationExpressionSymbolVisitor();
            symbol.Accept(visitor);
            this.CurrentMethod = visitor.MethodSymbol;
            this.MethodCollector.AddMethod(this.CurrentMethod, this.MethodAnalyzer.AnalyzeMethod(node));
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

        public override void VisitMemberAccessExpression(MemberAccessExpressionSyntax node)
        {
            if(this.CurrentMethod == null)
            {
                this.Visit(node.Expression);
                return;
            }
            var visitor = new InvocationExpressionSymbolVisitor();
            this.CurrentModel.GetSymbolInfo(node).Symbol.Accept(visitor);
            if(visitor.MethodSymbol!=null)
            {
                var calledType = this.CurrentModel.GetTypeInfo(node.Expression).Type;
                this.MethodCollector.AddMethod(visitor.MethodSymbol);
                this.CallCollector.AddCall(this.CurrentMethod,visitor.MethodSymbol,calledType);
            }
            this.Visit(node.Expression);
        }

        public override void VisitIdentifierName(IdentifierNameSyntax node)
        {
            if(this.CurrentMethod == null)
            {
                return;
            }
            var visitor = new InvocationExpressionSymbolVisitor();
            this.CurrentModel.GetSymbolInfo(node).Symbol.Accept(visitor);
            if(visitor.MethodSymbol!=null)
            {
                var calledType = visitor.MethodSymbol.ContainingType;
                this.MethodCollector.AddMethod(visitor.MethodSymbol);
                this.CallCollector.AddCall(this.CurrentMethod,visitor.MethodSymbol,calledType);
            }
            base.VisitIdentifierName(node);
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

    internal class InvocationExpressionSymbolVisitor : SymbolVisitor
    {
        public IMethodSymbol MethodSymbol { get; private set; }

        public override void VisitMethod(IMethodSymbol symbol)
        {
            this.MethodSymbol = symbol;
        }
    }
}
