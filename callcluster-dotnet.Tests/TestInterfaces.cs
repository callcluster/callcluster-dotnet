using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestInterfaces
    {
        [Fact]
        public async void TurnOnTurnableCallsAllImplementations()
        {
            CallgraphDTO dto = await Utils.Extract("interfaces/interfaces.csproj");
            string turnOnProgram = "interfaces.Program.TurnOnTurnable(interfaces.ITurnable)";
            string turnOnTurnable ="interfaces.ITurnable.TurnOn()";
            string turnOnMicrowave ="interfaces.Microwave.TurnOn()";
            string turnOnTelevision ="interfaces.Television.TurnOn()";
            CallgraphAssert.CallPresent(dto,turnOnProgram,turnOnTurnable);
            CallgraphAssert.CallPresent(dto,turnOnProgram,turnOnMicrowave);
            CallgraphAssert.CallPresent(dto,turnOnProgram,turnOnTelevision);
            CallgraphAssert.CallsFrom(dto,turnOnProgram,3);
        }

        [Fact]
        public async void BreakBreakableCallsAllImplementations()
        {
            CallgraphDTO dto = await Utils.Extract("interfaces/interfaces.csproj");
            string breakProgram = "interfaces.Program.BreakThings(interfaces.IBreakable)";
            CallgraphAssert.CallsFrom(dto, breakProgram, 6);//interface, tv, microwave, Delicate, Glass, Pottery
        }
    }
}