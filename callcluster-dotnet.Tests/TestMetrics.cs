using System;
using Xunit;
using callcluster_dotnet;
using callcluster_dotnet.dto;
using System.Linq;

namespace callcluster_dotnet.Tests
{
    [Collection("Sequential")]
    public class TestMetrics
    {
        [Fact]
        public async void LinesOk()
        {
            CallgraphDTO dto = await Utils.Extract("metrics/metrics.csproj");
            var oneLiner = Utils.Named(dto,"OneLiner()");
            Assert.Equal(3,oneLiner.linesOfCode);
            var twoLiner = Utils.Named(dto,"TwoLiner()");
            Assert.Equal(4,twoLiner.linesOfCode);
        }

        [Fact]
        public async void StatementsOk()
        {
            CallgraphDTO dto = await Utils.Extract("metrics/metrics.csproj");
            var oneLiner = Utils.Named(dto,"OneLiner()");
            Assert.Equal(3,oneLiner.numberOfStatements);
            var returner = Utils.Named(dto,"Returner()");
            Assert.Equal(2,returner.numberOfStatements);
        }

        [Fact]
        public async void ComplexityOk()
        {
            CallgraphDTO dto = await Utils.Extract("metrics/metrics.csproj");
            var oneLiner = Utils.Named(dto,"OneLiner()");
            Assert.Equal(1,oneLiner.cyclomaticComplexity);
            var returner = Utils.Named(dto,"Returner()");
            Assert.Equal(1,returner.cyclomaticComplexity);
            var ifonly = Utils.Named(dto,"IfOnly()");
            /*
            o
            | \
            o o
            | /
            o
            E=4
            N=4
            P=1
            M= E - N + 2P = 1
            */
            Assert.Equal(2,ifonly.cyclomaticComplexity);
            Assert.Equal(2,ifonly.basiliComplexity);

            var oneLess = Utils.Named(dto,"DoNormalComplexThingsWithOneLessBranch()");
            /*
            o
            |
            o
            | \
            | o
            | /
            o
            | \
            | o
            | /
            o
            |
            o
            E = 8
            N = 7
            M = E - N + 2 = 3
            */
            Assert.Equal(3,oneLess.cyclomaticComplexity);
            Assert.Equal(3,oneLess.basiliComplexity);
            var normal = Utils.Named(dto,"DoNormalComplexThings()");
            /*
            adds 3 edges and 2 vertices, so +1 branches
            */
            Assert.Equal(4,normal.cyclomaticComplexity);
            Assert.Equal(4,normal.basiliComplexity);

            var normalWithConditions = Utils.Named(dto,"DoNormalComplexThingsWithConditions()");
            /*
            adds 3 edges and 2 vertices, so +1 branches
            */
            Assert.Equal(6,normalWithConditions.cyclomaticComplexity);
            Assert.Equal(6,normalWithConditions.basiliComplexity);
        }
    }
}