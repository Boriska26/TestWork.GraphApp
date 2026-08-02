using Multicad;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Drawing;
using TestWork.GraphApp.Core.Algoritms;
using TestWork.GraphApp.Core.Models;
using TestWork.GraphApp.NCad.Extensions;
using TestWork.GraphApp.NCad.Objects;
using TestWork.GraphApp.NCad.Services;
using GraphNode = TestWork.GraphApp.NCad.Objects.GraphNode;

namespace TestWork.GraphApp.NCad.Commands
{

    public class GraphCommand
    {
        private readonly GraphQuery _graphQuery = new GraphQuery();
        private readonly GraphObjectFactory _graphObjectFactory;
        private readonly GraphBuilder _graphBuilder;

        public GraphCommand()
        {
            _graphObjectFactory = new GraphObjectFactory(_graphQuery);
            _graphBuilder = new GraphBuilder(_graphQuery, _graphObjectFactory);
        }

        [CommandMethod("TW_FINDPATH", CommandFlags.NoCheck)]
        public void FindPath()
        {
            GraphNode startNode = SelectSingleNode("Выберите первый узел: ");
            if (startNode == null) return;

            GraphNode goalNode = SelectSingleNode("Выберите второй узел: ");
            if (goalNode == null) return;

            Graph graph = BuildGraphFromDrawing();

            var finder = new DeykstraPathFinder();
            IReadOnlyList<Guid> path = finder.Find(graph, startNode.NodeId, goalNode.NodeId);

            if (path.Count == 0)
            {
                return;
            }

            HighlightPath(path);
        }

        private GraphNode SelectSingleNode(string prompt)
        {
            McObjectId id = McObjectManager.SelectObject(prompt);

            return id.GetObject() as GraphNode;
        }

        private void HighlightPath(IReadOnlyList<Guid> path)
        {
            var edgeFilter = ObjectFilter.Create(true);
            edgeFilter.AddType(typeof(GraphEdge));
            var edgeIds = McObjectManager.SelectObjects(edgeFilter);

            for (int i = 0; i < path.Count - 1; i++)
            {
                Guid a = path[i];
                Guid b = path[i + 1];

                foreach (var id in edgeIds)
                {
                    if (id.GetObject() is GraphEdge edge &&
                        edge.IsIncidentTo(a) && edge.IsIncidentTo(b))
                    {
                        edge.TryModify(1);
                        edge.DbEntity.Color = Color.Lime;
                        edge.DbEntity.Update();
                        break;
                    }
                }
            }
        }

        [CommandMethod("TW_BUILD_CIRCLE", CommandFlags.NoCheck)]
        public void BuildGraphCircle()
        {
            _graphBuilder.Build(NodeShape.CircleBlue);
        }

        [CommandMethod("TW_BUILD_TRIANGLE", CommandFlags.NoCheck)]
        public void BuildGraphTriangle()
        {
            _graphBuilder.Build(NodeShape.TriangleRed);
        }

        [CommandMethod("TW_EDGESTYLE", CommandFlags.NoCheck)]
        public void SetEdgesStyle()
        {
            GraphEdge? reference = SelectSingleEdge("Выберите ребро образец: ");
            if (reference == null)
            {
                return;
            }

            Color color = reference.DbEntity.Color;
            int lineWeight = reference.DbEntity.LineWeight;
            int lineType = reference.DbEntity.LineType;

            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphEdge));
            var ids = McObjectManager.SelectObjects(filter);

            foreach (var id in ids)
            {
                if (id.GetObject() is GraphEdge edge)
                {
                    edge.TryModify(1);
                    edge.DbEntity.Color = color;
                    edge.DbEntity.LineWeight = lineWeight;
                    edge.DbEntity.LineType = lineType;
                    edge.DbEntity.Update();
                }
            }
        }

        private GraphEdge? SelectSingleEdge(string prompt)
        {
            McObjectId id = McObjectManager.SelectObject(prompt);

            if (id.IsNull)
            {
                return null;
            }

            return id.GetObject() as GraphEdge;
        }

        private Graph BuildGraphFromDrawing()
        {
            var graph = new Graph();

            var nodePositions = new Dictionary<Guid, Point3d>();

            var nodeFilter = ObjectFilter.Create(true);
            nodeFilter.AddType(typeof(GraphNode));
            foreach (var id in McObjectManager.SelectObjects(nodeFilter))
            {
                if (id.GetObject() is GraphNode node)
                {
                    graph.AddNode(node.NodeId);
                    nodePositions[node.NodeId] = node.Position;
                }
            }

            var edgeFilter = ObjectFilter.Create(true);
            edgeFilter.AddType(typeof(GraphEdge));
            foreach (var id in McObjectManager.SelectObjects(edgeFilter))
            {
                if (id.GetObject() is GraphEdge edge)
                {
                    if (nodePositions.TryGetValue(edge.FirstNodeId, out var p1) &&
                        nodePositions.TryGetValue(edge.SecondNodeId, out var p2))
                    {
                        double length = p1.DistanceTo(p2);
                        graph.AddEdge(edge.FirstNodeId, edge.SecondNodeId, length);
                    }
                }
            }

            return graph;
        }
    }
}