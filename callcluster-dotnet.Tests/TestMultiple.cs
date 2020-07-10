using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestMultiple
    {
        [Fact]
        public async void AssemblysAreUnique()
        {
            CallgraphDTO dto = await Utils.Extract("multiple/multiple.sln");
            var assemblyNames = dto.community.communities.Select(c=>c.name);
            Assert.Equal(assemblyNames.Distinct().Count(),assemblyNames.Count());
        }

        [Fact]
        public async void IntroversionIsFound()
        {
            CallgraphDTO dto = await Utils.Extract("multiple/multiple.sln");
            var subClass = dto.community
            ?.Child("consotest")
            ?.Child("consotest.dll")
            ?.Child("")
            ?.Child("introspection")
            ?.Child("Outside")
            ?.Child("Inside")
            ?.Child("Within")
            ?.Child("DepperWithin");
            Assert.NotNull(subClass);
            Assert.Equal(1,subClass.functions.Count());
        }

    }
}