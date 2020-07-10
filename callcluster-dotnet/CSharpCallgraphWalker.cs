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
        private IMethodCollector MethodCollector;
        private IClassCollector ClassCollector;
        private ICSharpMethodAnalyzer MethodAnalyzer;

        public CSharpCallgraphWalker(IMethodCollector methodCollector, IClassCollector classCollector, ICSharpMethodAnalyzer methodAnalyzer):base(SyntaxWalkerDepth.Node)
        {
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
            this.MethodCollector.AddMethod(this.CurrentMethod, this.MethodAnalyzer.AnalyzeMethod(node, this.CurrentModel),this.CurrentModel.GetOperation(node));
        }

        public override void VisitMethodDeclaration(MethodDeclarationSyntax node)
        {
            VisitBaseMethodDeclarationSyntax(node);
            base.VisitMethodDeclaration(node);
        }

        public override void VisitConstructorDeclaration(ConstructorDeclarationSyntax node)
        {
            VisitBaseMethodDeclarationSyntax(node);
            base.VisitConstructorDeclaration(node);
        }

        public override void VisitConversionOperatorDeclaration(ConversionOperatorDeclarationSyntax node)
        {
            VisitBaseMethodDeclarationSyntax(node);
            base.VisitConversionOperatorDeclaration(node);
        }

        public override void VisitDestructorDeclaration(DestructorDeclarationSyntax node)
        {
            VisitBaseMethodDeclarationSyntax(node);
            base.VisitDestructorDeclaration(node);
        }

        public override void VisitOperatorDeclaration(OperatorDeclarationSyntax node)
        {
            VisitBaseMethodDeclarationSyntax(node);
            base.VisitOperatorDeclaration(node);
        }

    }
}
