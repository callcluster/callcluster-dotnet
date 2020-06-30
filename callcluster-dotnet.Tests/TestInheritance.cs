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
            string turnOnCopter = "complex_project.Program.TurnOnCopter(complex_project.Helicopter)";
            string turnOnMotor = "complex_project.MotorizedTransport.TurnOn()";
            CallgraphAssert.CallPresent(dto,turnOnCopter,turnOnMotor);
            CallgraphAssert.CallsFrom(dto,turnOnCopter,1);
        }

        [Fact]
        public async void CarCanBeLambo()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            string turnOnCar = "complex_project.Program.TurnOnCar(complex_project.Car)";
            string turnOnMotor = "complex_project.MotorizedTransport.TurnOn()";
            string turnOnLambo = "complex_project.Lambo.TurnOn()";
            CallgraphAssert.CallPresent(dto,turnOnCar,turnOnMotor);
            CallgraphAssert.CallPresent(dto,turnOnCar,turnOnLambo);
            CallgraphAssert.CallsFrom(dto,turnOnCar,2);
        }

        [Fact]
        public async void OverridenCallsSpecific()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            string turnOnProgram = "complex_project.Program.TurnOnLambo(complex_project.Lambo)";
            string turnOnLambo = "complex_project.Lambo.TurnOn()";
            CallgraphAssert.CallsFrom(dto,turnOnProgram);
            CallgraphAssert.CallPresent(dto,turnOnProgram,turnOnLambo);
        }

        [Fact]
        public async void GeneralCallsBoth()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            string turnOnProgram = "complex_project.Program.TurnOnTransport(complex_project.MotorizedTransport)";// 9
            string turnOnLambo = "complex_project.Lambo.TurnOn()";// 3
            string turnOnMotor = "complex_project.MotorizedTransport.TurnOn()";// 2
            CallgraphAssert.CallsFrom(dto,turnOnProgram,2);
            CallgraphAssert.CallPresent(dto,turnOnProgram,turnOnLambo);
            CallgraphAssert.CallPresent(dto,turnOnProgram,turnOnMotor);
        }
    }
}
