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
            ?.Community("consotest")
            ?.Community("consotest.dll")
            ?.Community("")
            ?.Community("introspection")
            ?.Community("Outside")
            ?.Community("Inside")
            ?.Community("Within")
            ?.Community("DepperWithin");
            Assert.NotNull(subClass);
            Assert.Equal(1,subClass.functions.Count());
        }

        [Fact]
        public async void AllHiExist()
        {
            CallgraphDTO dto = await Utils.Extract("multiple/multiple.sln");
            var libra = dto.community
            ?.Community("libratest")
            ?.Community("libratest.dll")
            ?.Community("")
            ?.Community("hello")
            ?.Community("all")
            ?.Community("good")
            ?.Community("Hi");
            Assert.NotNull(libra);

            var servelibra = dto.community
            ?.Community("servelibratest")
            ?.Community("servelibratest.dll")
            ?.Community("")
            ?.Community("hello")
            ?.Community("all")
            ?.Community("good")
            ?.Community("Hi");
            Assert.NotNull(servelibra);
            
        }

        [Fact]
        public async void NamespacesAreWithinEachOther()
        {
            CallgraphDTO dto = await Utils.Extract("multiple/multiple.sln");
            var helloNs = dto.community
            ?.Community("servelibratest")
            ?.Community("servelibratest.dll")
            ?.Community("")
            ?.Community("hello");
            Assert.NotNull(helloNs);
            Assert.NotNull(helloNs.Community("Addressing"));
            Assert.NotEmpty(helloNs.Community("Addressing").functions);

            var goodbye = helloNs
            ?.Community("all")
            ?.Community("Sad")
            ?.Community("Goodbye");
            Assert.NotNull(goodbye);
            Assert.NotEmpty(goodbye.functions);

            var hi = helloNs
            ?.Community("all")
            ?.Community("good")
            ?.Community("Hi");
            Assert.NotNull(hi);
            Assert.NotEmpty(hi.functions);
            
        }


    }
}