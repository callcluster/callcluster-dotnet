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
            string main = "constructors.Main.Main()";
            string doThis="constructors.Main.DoThis()";
            string doThat="constructors.Main.DoThat()";
            CallgraphAssert.CallPresent(dto,main,doThis);
            CallgraphAssert.CallPresent(dto,main,doThat);
            CallgraphAssert.CallsFrom(dto,main,2);
        }

        [Fact]
        public async void DoThatCallsThreeConstructors()
        {
            CallgraphDTO dto = await Utils.Extract("constructors/constructors.csproj");
            string main = "constructors.Main.Main()";
            string mainString = "constructors.Main.Main(string, string)";
            string mainInt ="constructors.Main.Main(int, int)";
            string doThat="constructors.Main.DoThat()";
            CallgraphAssert.CallPresent(dto,doThat,main);
            CallgraphAssert.CallPresent(dto,doThat,mainString);
            CallgraphAssert.CallPresent(dto,doThat,mainInt);
            CallgraphAssert.CallsFrom(dto,doThat,3);
        }

        [Fact]
        public async void GrandsonConstructorCallsParent()
        {
            CallgraphDTO dto = await Utils.Extract("constructors/constructors.csproj");
            string grandson = "constructors.Grandson.Grandson()";
            string child = "constructors.Child.Child(int)";
            string mainString = "constructors.Main.Main(string, string)";
            CallgraphAssert.CallPresent(dto,grandson,child);
            CallgraphAssert.CallPresent(dto,grandson,mainString);
            CallgraphAssert.CallsFrom(dto,grandson,2);
        }

    }
}