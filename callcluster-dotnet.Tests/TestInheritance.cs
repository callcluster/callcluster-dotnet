using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestInheritance
    {
        [Fact]
        public async void NotOverridenOnlyCallsBase()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            CommunityDTO project = dto.community.Namespace("complex_project");

            long? turnOnCopterId = project
            ?.Community("Program")
            ?.Function("TurnOnCopter(Helicopter)",dto);
            Assert.NotNull(turnOnCopterId);

            long? turnOnMotorId = project
            ?.Community("MotorizedTransport")
            ?.Function("TurnOn()",dto);
            Assert.NotNull(turnOnMotorId);

            var children = dto.community.communities.ToList();

            CallgraphAssert.CallPresent(dto,turnOnCopterId.Value,turnOnMotorId.Value);
            CallgraphAssert.CallsFrom(dto,turnOnCopterId.Value,1);
        }

        [Fact]
        public async void CarCanBeLambo()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            CommunityDTO project = dto.community.Namespace("complex_project");

            long? turnOnCarId = project
            ?.Community("Program")
            ?.Function("TurnOnCar(Car)", dto);
            Assert.NotNull(turnOnCarId);

            long? turnOnMotorId = project
            ?.Community("MotorizedTransport")
            ?.Function("TurnOn()", dto);
            Assert.NotNull(turnOnMotorId);
            
            long? turnOnLamboId = project
            ?.Community("Lambo")
            ?.Function("TurnOn()", dto);
            Assert.NotNull(turnOnLamboId);

            CallgraphAssert.CallPresent(dto,turnOnCarId.Value,turnOnMotorId.Value);
            CallgraphAssert.CallPresent(dto,turnOnCarId.Value,turnOnLamboId.Value);
            CallgraphAssert.CallsFrom(dto,turnOnCarId.Value,2);
        }

        [Fact]
        public async void WaterVehicleCanBeShip()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            CommunityDTO project = dto.community.Namespace("complex_project");

            long? turnOnProgramId = project
            ?.Community("Program")
            ?.Function("TurnOnWaterVehicle(WaterVehicle)",dto);
            Assert.NotNull(turnOnProgramId);

            long? turnOnWaterVehicleId = project
            ?.Community("WaterVehicle")
            ?.Function("TurnOn()",dto);
            Assert.NotNull(turnOnWaterVehicleId);
            
            long? turnOnSmallShipId = project
            ?.Community("SmallShip")
            ?.Function("TurnOn()",dto);
            Assert.NotNull(turnOnSmallShipId);
            
            long? turnOnBigShipId = project
            ?.Community("BigShip")
            ?.Function("TurnOn()",dto);
            Assert.NotNull(turnOnBigShipId);

            
            CallgraphAssert.CallPresent(dto,
                turnOnProgramId.Value,
                turnOnSmallShipId.Value
            );
            CallgraphAssert.CallPresent(dto,
                turnOnProgramId.Value,
                turnOnBigShipId.Value
            );
            CallgraphAssert.CallPresent(dto,
                turnOnProgramId.Value,
                turnOnWaterVehicleId.Value
            );
            CallgraphAssert.CallsFrom(dto,turnOnProgramId.Value,3);
        }

        [Fact]
        public async void OverridenCallsSpecific()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            CommunityDTO project = dto.community.Namespace("complex_project");
            long? turnOnProgram = project
            ?.Community("Program")
            ?.Function("TurnOnLambo(Lambo)", dto);

            long? turnOnLambo = project
            ?.Community("Lambo")
            ?.Function("TurnOn()", dto);

            CallgraphAssert.CallsFrom(dto,turnOnProgram.Value);
            CallgraphAssert.CallPresent(dto, turnOnProgram.Value, turnOnLambo.Value);
        }

        [Fact]
        public async void GeneralCallsAll()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            CommunityDTO project = dto.community.Namespace("complex_project");
            long? turnOnProgram = project
            ?.Community("Program")
            ?.Function("TurnOnTransport(MotorizedTransport)", dto);
            
            long? turnOnLambo = project
            ?.Community("Lambo")
            ?.Function("TurnOn()", dto);

            long? turnOnMotor = project
            ?.Community("MotorizedTransport")
            ?.Function("TurnOn()", dto);

            CallgraphAssert.CallsFrom(dto,turnOnProgram.Value,5);
            CallgraphAssert.CallPresent(dto,turnOnProgram.Value,turnOnLambo.Value);
            CallgraphAssert.CallPresent(dto,turnOnProgram.Value,turnOnMotor.Value);
        }
    }
}
