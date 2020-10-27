using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestComplex
    {
        [Fact]
        public async void Triangle()
        {
            CallgraphDTO dto = await Utils.Extract("complex-project/complex-project.csproj");
            long oneId = Utils.IndexOf(dto,"One()");
            long twoId = Utils.IndexOf(dto,"Two()");
            long threeId = Utils.IndexOf(dto,"Three()");
            Assert.Contains(new CallDTO(){ from = oneId, to = twoId },dto.calls,Utils.CallComparer);
            Assert.Contains(new CallDTO(){ from = oneId, to = threeId },dto.calls,Utils.CallComparer);
            Assert.Contains(new CallDTO(){ from = twoId, to = threeId },dto.calls,Utils.CallComparer);
        }

        [Fact]
        public async void Cycle()
        {
            CallgraphDTO dto = await Utils.Extract("complex-project/complex-project.csproj");
            long head = Utils.IndexOf(dto,"LoopHead()");
            long tail = Utils.IndexOf(dto,"LoopTail()");
            Assert.Contains(new CallDTO(){ from = head, to = tail },dto.calls,Utils.CallComparer);
            Assert.Contains(new CallDTO(){ from = tail, to = head },dto.calls,Utils.CallComparer);
        }
    }
}
