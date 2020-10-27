using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestCallCluster
    {
        [Fact]
        public async void CallsToGenericMethods()
        {
            CallgraphDTO dto = await Utils.Extract("../callcluster-dotnet.sln");
            CommunityDTO project = dto.community.Namespace("callcluster_dotnet");
            
            long? addClassMethod = project
            ?.Community("CallgraphCollector")
            ?.Function("AddClass(INamedTypeSymbol)",dto);

            long? addTreeMethod = project
            ?.Community("Tree")
            ?.Function("Add(T, T)",dto);

            CallgraphAssert.CallPresent(dto,addClassMethod.Value,addTreeMethod.Value);
        }
    }
}
