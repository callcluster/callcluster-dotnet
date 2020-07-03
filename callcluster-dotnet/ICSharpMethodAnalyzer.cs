using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace callcluster_dotnet
{
    internal interface ICSharpMethodAnalyzer 
    {
        MethodAnalysisData AnalyzeMethod(BaseMethodDeclarationSyntax syntax, SemanticModel model);
        MethodAnalysisData AnalyzeMethod(LocalFunctionStatementSyntax node, SemanticModel model);
    }
}