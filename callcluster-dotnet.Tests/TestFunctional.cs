using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestFunctional
    {
        [Fact]
        public async void ReferencesDectected()
        {
            CallgraphDTO dto = await Utils.Extract("functional/functional.csproj");
            string mapNumbers = "MapNumbers()";
            string method = "AsNumbersMethod(Int32, Int32)";
            string func = "AsNumberPrivate(Int32)";
            string transform = "Transform(Int32, Int32)";
            string newThing = "Thing()";
            CallgraphAssert.CallPresent(dto,mapNumbers,method);
            CallgraphAssert.CallPresent(dto,mapNumbers,func);
            CallgraphAssert.CallPresent(dto,mapNumbers,transform);
            CallgraphAssert.CallPresent(dto,mapNumbers,newThing);
        }


    }
}