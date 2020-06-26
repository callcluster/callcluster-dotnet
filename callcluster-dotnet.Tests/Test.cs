using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;

namespace callcluster_dotnet.Tests
{
    public class Test
    {
        [Fact]
        public async void SimpleSolution()
        {
            CallgraphDTO dto = await callcluster_dotnet.Program.Extract("./test-solutions/cli-sln/cli-sln.sln");
        }
    }
}
