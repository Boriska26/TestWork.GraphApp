using TestWork.GraphApp.Core.Algoritms;
using TestWork.GraphApp.Core.Models;

namespace TestWork.GraphApp.Core.Tests
{
    public class DeykstraPathFinderTests
    {
        [Fact]
        public void FindPath_DirectEdge_ReturnsBothNodes()
        {
           DeykstraPathFinder finder = new DeykstraPathFinder();

            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var graph = new Graph();
            graph.AddEdge(a, b, 1.0);

            var path = finder.Find(graph, a, b);

            Assert.Equal(new[] { a, b }, path);
        }

        [Fact]
        public void FindPath_ChoosesShorterRoute()
        {
            DeykstraPathFinder finder = new DeykstraPathFinder();
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var c = Guid.NewGuid();
            var graph = new Graph();
            graph.AddEdge(a, b, 1.0);
            graph.AddEdge(b, c, 1.0);
            graph.AddEdge(a, c, 5.0);

            var path = finder.Find(graph, a, c);

            Assert.Equal(new[] { a, b, c }, path);
        }

        [Fact]
        public void FindPath_SameStartAndGoal_ReturnsSingleNode()
        {
            DeykstraPathFinder finder = new DeykstraPathFinder();
            var a = Guid.NewGuid();
            var graph = new Graph();
            graph.AddNode(a);

            var path = finder.Find(graph, a, a);

            Assert.Equal(new[] { a }, path);
        }

        [Fact]
        public void FindPath_NoConnection_ReturnsEmpty()
        {
            DeykstraPathFinder finder = new DeykstraPathFinder();
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var graph = new Graph();
            graph.AddNode(a);
            graph.AddNode(b);

            var path = finder.Find(graph, a, b);

            Assert.Empty(path);
        }

        [Fact]
        public void FindPath_IsUndirected_WorksBothDirections()
        {
            DeykstraPathFinder finder = new DeykstraPathFinder();
            var a = Guid.NewGuid();
            var b = Guid.NewGuid();
            var graph = new Graph();
            graph.AddEdge(a, b, 1.0);

            var path = finder.Find(graph, b, a);

            Assert.Equal(new[] { b, a }, path);
        }
    }
}
