using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    public class Test
    {
        [Fact]
        public async void SimpleProjectHasWriteLine()
        {
            CallgraphDTO dto = await callcluster_dotnet.Program.Extract("../../../../test-solutions/cli-csproj/cli-csproj.csproj");
            Assert.Contains(dto.functions,f => f.name=="System.Console.WriteLine(string)");
        }

        [Fact]
        public async void SimpleProjectHasOneCall()
        {
            CallgraphDTO dto = await callcluster_dotnet.Program.Extract("../../../../test-solutions/cli-csproj/cli-csproj.csproj");
            Assert.Single(dto.calls);
        }
    }
}
