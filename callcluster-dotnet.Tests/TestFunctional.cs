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
            string mapNumbers = "functional.Program.MapNumbers()";
            string method = "functional.Program.AsNumbersMethod(int, int)";
            string func = "AsNumberPrivate(int)";
            string transform = "functional.Thing.Transform(int, int)";
            string newThing = "functional.Thing.Thing()";
            CallgraphAssert.CallPresent(dto,mapNumbers,method);
            CallgraphAssert.CallPresent(dto,mapNumbers,func);
            CallgraphAssert.CallPresent(dto,mapNumbers,transform);
            CallgraphAssert.CallPresent(dto,mapNumbers,newThing);
        }


    }
}