using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Operations;

namespace callcluster_dotnet
{
    internal class MethodWalker : OperationWalker
    {
        private ICallCollector Collector;
        private IMethodSymbol Caller;

        public MethodWalker(ICallCollector collector, IMethodSymbol caller)
        {
            this.Collector = collector;
            this.Caller = caller;
        }

        public override void VisitMethodReference(IMethodReferenceOperation operation)
        {
            Collector.AddCall(Caller,operation.Method,operation.Instance?.Type ?? operation.Method.ContainingType);
            base.VisitMethodReference(operation);
        }

        public override void VisitInvocation(IInvocationOperation operation)
        {
            Collector.AddCall(Caller,operation.TargetMethod,operation.Instance?.Type ?? operation.TargetMethod.ContainingType);
            base.VisitInvocation(operation);
        }

        public override void VisitObjectCreation(IObjectCreationOperation operation)
        {
            Collector.AddCall(Caller,operation.Constructor,operation.Constructor.ContainingType);
            base.VisitObjectCreation(operation);
        }
    }
}