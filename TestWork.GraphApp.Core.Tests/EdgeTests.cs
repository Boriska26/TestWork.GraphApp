using TestWork.GraphApp.Core.Exceptions;
using TestWork.GraphApp.Core.Models;

namespace TestWork.GraphApp.Core.Tests
{
    public class EdgeTests
    {
        [Fact]
        public void IsIncidentTo_ReturnsTrue_ForBothEndpoints()
        {
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var edge = new Edge(a, b);

            Assert.True(edge.IsIncidentTo(a));
            Assert.True(edge.IsIncidentTo(b));
        }

        [Fact]
        public void IsIncidentTo_ReturnsFalse_ForUnrelatedNode()
        {
            var edge = new Edge(Guid.NewGuid(), Guid.NewGuid());
            Assert.False(edge.IsIncidentTo(Guid.NewGuid()));
        }

        [Fact]
        public void GetOtherNode_ReturnsOppositeEndpoint()
        {
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var edge = new Edge(a, b);

            Assert.Equal(b, edge.GetOtherNode(a));
            Assert.Equal(a, edge.GetOtherNode(b));
        }

        [Fact]
        public void Constructor_Throws_ForSelfLoop()
        {
            var a = Guid.NewGuid();
            Assert.Throws<EdgeException>(() => new Edge(a, a));
        }
    }
}
