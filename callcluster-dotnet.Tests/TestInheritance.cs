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
            long turnOnCopter = Utils.IndexOf(dto,"complex_project.Program.TurnOnCopter(complex_project.Helicopter)");
            long turnOnMotor = Utils.IndexOf(dto,"complex_project.MotorizedTransport.TurnOn()");
            Assert.Single(dto.calls.Where(c=>c.from==turnOnCopter));
            Assert.Single(dto.calls.Where(c=>c.from==turnOnCopter && c.to==turnOnMotor));
        }

        [Fact]
        public async void OverridenCallsSpecific()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            long turnOnProgram = Utils.IndexOf(dto,"complex_project.Program.TurnOnLambo(complex_project.Lambo)");
            long turnOnLambo = Utils.IndexOf(dto,"complex_project.Lambo.TurnOn()");
            Assert.Single(dto.calls.Where(c=>c.from==turnOnProgram));
            Assert.Single(dto.calls.Where(c=>c.from==turnOnProgram && c.to==turnOnLambo));
        }

        [Fact]
        public async void GeneralCallsBoth()
        {
            CallgraphDTO dto = await Utils.Extract("inheritance/complex-project.csproj");
            long turnOnProgram = Utils.IndexOf(dto,"complex_project.Program.TurnOnTransport(complex_project.Lambo)");
            long turnOnLambo = Utils.IndexOf(dto,"complex_project.Lambo.TurnOn()");
            long turnOnMotor = Utils.IndexOf(dto,"complex_project.MotorizedTransport.TurnOn()");
            Assert.True(dto.calls.Where(c=>c.from==turnOnProgram).ToList().Count==2);
            Assert.Single(dto.calls.Where(c=>c.from==turnOnProgram && c.from==turnOnLambo));
            Assert.Single(dto.calls.Where(c=>c.from==turnOnProgram && c.from==turnOnMotor));
        }
    }
}
