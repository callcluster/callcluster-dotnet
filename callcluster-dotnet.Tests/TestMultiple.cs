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

        [Fact]
        public async void AllHiExist()
        {
            CallgraphDTO dto = await Utils.Extract("multiple/multiple.sln");
            var libra = dto.community
            ?.Child("libratest")
            ?.Child("libratest.dll")
            ?.Child("")
            ?.Child("hello")
            ?.Child("all")
            ?.Child("good")
            ?.Child("Hi");
            Assert.NotNull(libra);

            var servelibra = dto.community
            ?.Child("servelibratest")
            ?.Child("servelibratest.dll")
            ?.Child("")
            ?.Child("hello")
            ?.Child("all")
            ?.Child("good")
            ?.Child("Hi");
            Assert.NotNull(servelibra);
            
        }

        [Fact]
        public async void NamespacesAreWithinEachOther()
        {
            CallgraphDTO dto = await Utils.Extract("multiple/multiple.sln");
            var helloNs = dto.community
            ?.Child("servelibratest")
            ?.Child("servelibratest.dll")
            ?.Child("")
            ?.Child("hello");
            Assert.NotNull(helloNs);
            Assert.NotNull(helloNs.Child("Addressing"));
            Assert.NotEmpty(helloNs.Child("Addressing").functions);

            var goodbye = helloNs
            ?.Child("all")
            ?.Child("Sad")
            ?.Child("Goodbye");
            Assert.NotNull(goodbye);
            Assert.NotEmpty(goodbye.functions);

            var hi = helloNs
            ?.Child("all")
            ?.Child("good")
            ?.Child("Hi");
            Assert.NotNull(hi);
            Assert.NotEmpty(hi.functions);
            
        }


    }
}