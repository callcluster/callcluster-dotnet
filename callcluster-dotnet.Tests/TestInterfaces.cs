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
            CommunityDTO project = dto.community.Namespace("interfaces");

            long? turnOnProgram = project
            ?.Community("Program")
            ?.Function("TurnOnTurnable(ITurnable)", dto);
            
            long? turnOnTurnable = project
            ?.Community("ITurnable")
            ?.Function("TurnOn()", dto);
            
            long? turnOnMicrowave = project
            ?.Community("Microwave")
            ?.Function("TurnOn()", dto);

            long? turnOnTelevision = project
            ?.Community("Television")
            ?.Function("TurnOn()", dto);
            
            CallgraphAssert.CallPresent(dto,turnOnProgram.Value,turnOnTurnable.Value);
            CallgraphAssert.CallPresent(dto,turnOnProgram.Value,turnOnMicrowave.Value);
            CallgraphAssert.CallPresent(dto,turnOnProgram.Value,turnOnTelevision.Value);
            CallgraphAssert.CallsFrom(dto,turnOnProgram.Value,3);
        }

        [Fact]
        public async void BreakBreakableCallsAllImplementations()
        {
            CallgraphDTO dto = await Utils.Extract("interfaces/interfaces.csproj");
            long? breakProgram = dto.community
            ?.Namespace("interfaces")
            ?.Community("Program")
            ?.Function("BreakThings(IBreakable)", dto);

            CallgraphAssert.CallsFrom(dto, breakProgram.Value, 6);//interface, tv, microwave, Delicate, Glass, Pottery
        }
    }
}