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
            long oneId = Utils.IndexOf(dto,"complex_project.Program.One()");
            long twoId = Utils.IndexOf(dto,"complex_project.Program.Two()");
            long threeId = Utils.IndexOf(dto,"complex_project.Program.Three()");
            Assert.Contains(new CallDTO(){ from = oneId, to = twoId },dto.calls,Utils.CallComparer);
            Assert.Contains(new CallDTO(){ from = oneId, to = threeId },dto.calls,Utils.CallComparer);
            Assert.Contains(new CallDTO(){ from = twoId, to = threeId },dto.calls,Utils.CallComparer);
        }

        [Fact]
        public async void Cycle()
        {
            CallgraphDTO dto = await Utils.Extract("complex-project/complex-project.csproj");
            long head = Utils.IndexOf(dto,"complex_project.Program.LoopHead()");
            long tail = Utils.IndexOf(dto,"complex_project.Program.LoopTail()");
            Assert.Contains(new CallDTO(){ from = head, to = tail },dto.calls,Utils.CallComparer);
            Assert.Contains(new CallDTO(){ from = tail, to = head },dto.calls,Utils.CallComparer);
        }
    }
}
