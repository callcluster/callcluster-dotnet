using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestClis
    {
        [Theory]
        [InlineData("cli-csproj/cli-csproj.csproj")]
        [InlineData("cli-sln/cli-sln.sln")]
        public async void SimpleProjectHasWriteLine(string value)
        {
            CallgraphDTO dto = await Utils.Extract(value);
            Assert.Contains(dto.functions,f => f.name=="System.Console.WriteLine(string)");
        }

        [Theory]
        [InlineData("cli-csproj/cli-csproj.csproj")]
        [InlineData("cli-sln/cli-sln.sln")]
        public async void SimpleProjectHasOneCall(string value)
        {
            CallgraphDTO dto = await Utils.Extract(value);
            Assert.Single(dto.calls);
        }
    }
}
