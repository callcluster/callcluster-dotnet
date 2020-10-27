using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestConstructors
    {
        [Fact]
        public async void MainConstructorCallsMethods()
        {
            CallgraphDTO dto = await Utils.Extract("constructors/constructors.csproj");
            string main = "Main()";
            string doThis="DoThis()";
            string doThat="DoThat()";
            CallgraphAssert.CallPresent(dto,main,doThis);
            CallgraphAssert.CallPresent(dto,main,doThat);
            CallgraphAssert.CallsFrom(dto,main,2);
        }

        [Fact]
        public async void DoThatCallsThreeConstructors()
        {
            CallgraphDTO dto = await Utils.Extract("constructors/constructors.csproj");
            string main = "Main()";
            string mainString = "Main(String, String)";
            string mainInt ="Main(Int32, Int32)";
            string doThat="DoThat()";
            CallgraphAssert.CallPresent(dto,doThat,main);
            CallgraphAssert.CallPresent(dto,doThat,mainString);
            CallgraphAssert.CallPresent(dto,doThat,mainInt);
            CallgraphAssert.CallsFrom(dto,doThat,3);
        }

        [Fact]
        public async void GrandsonConstructorCallsParent()
        {
            CallgraphDTO dto = await Utils.Extract("constructors/constructors.csproj");
            string grandson = "Grandson()";
            string child = "Child(Int32)";
            string mainString = "Main(String, String)";
            CallgraphAssert.CallPresent(dto,grandson,child);
            CallgraphAssert.CallPresent(dto,grandson,mainString);
            CallgraphAssert.CallsFrom(dto,grandson,2);
        }

    }
}