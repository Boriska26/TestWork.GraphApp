using Multicad;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Drawing;
using TestWork.GraphApp.Core.Algoritms;
using TestWork.GraphApp.Core.Models;
using TestWork.GraphApp.NCad.Objects;
using GraphNode = TestWork.GraphApp.NCad.Objects.GraphNode;

namespace TestWork.GraphApp.NCad.Commands
{
    public class GraphCommand
    {
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

        [CommandMethod("TW_GRAPHNODE", CommandFlags.NoCheck)]
        public void CreateNode()
        {
            var jig = new InputJig();
            InputResult res = jig.GetPoint("Выберите точку для узла: ");
            if (res.Result != InputResult.ResultCode.Normal)
            {
                return;
            }

            CreateNode(res.Point);
        }

        [CommandMethod("TW_BUILD_CIRCLE", CommandFlags.NoCheck)]
        public void BuildGraphCircle()
        {
            BuildGraph(NodeShape.CircleBlue);
        }

        [CommandMethod("TW_BUILD_TRIANGLE", CommandFlags.NoCheck)]
        public void BuildGraphTriangle()
        {
            BuildGraph(NodeShape.TriangleRed);
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

        private GraphNode CreateNode(Point3d position)
        {
            var node = new GraphNode
            {
                Position = position,
                Shape = NodeShape.CircleBlue
            };
            node.DbEntity.AddToCurrentDocument();

            return node;
        }

        private GraphEdge CreateEdge(Guid firstNode, Guid secondNode)
        {
            var edge = new GraphEdge(firstNode, secondNode);
            edge.DbObject.AddToCurrentDocument();

            return edge;
        }

        private GraphNode FindNodeNear(Point3d point, double captureRadius, Guid excludeNodeId)
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphNode));
            var ids = McObjectManager.SelectObjects(filter);

            foreach (var id in ids)
            {
                if (id.GetObject() is GraphNode node
                    && node.NodeId != excludeNodeId
                    && node.Position.DistanceTo(point) <= captureRadius)
                {
                    return node;
                }
            }

            return null;
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

        private GraphNode CreateNode(Point3d position, NodeShape shape)
        {
            var node = new GraphNode { Position = position, Shape = shape };
            node.DbEntity.AddToCurrentDocument();

            return node;
        }

        private void BuildGraph(NodeShape shape)
        {
            GraphNode previousNode = null;

            while (true)
            {
                Point3d startPos = previousNode?.Position ?? Point3d.Origin;
                GraphNode ghostNode = CreateNode(startPos, shape);

                GraphEdge ghostEdge = null;
                if (previousNode != null)
                {
                    ghostEdge = CreateEdge(previousNode.NodeId, ghostNode.NodeId);
                }

                var jig = new InputJig();
                jig.ExcludeObject(ghostNode.ID);
                if (ghostEdge != null)
                {
                    jig.ExcludeObject(ghostEdge.ID);
                }

                jig.MouseMove = (a, s) =>
                {
                    ghostNode.TryModify(1);
                    ghostNode.Position = s.Point;
                    ghostNode.DbEntity.Update();

                    if (ghostEdge != null)
                    {
                        ghostEdge.TryModify(1);
                        ghostEdge.DbEntity.Update();
                    }
                };

                InputResult res = jig.GetPoint("Выберите точку для узла (Esc — завершить): ");

                if (res.Result != InputResult.ResultCode.Normal)
                {
                    ghostNode.DbEntity.Erase();
                    ghostEdge?.DbEntity.Erase();

                    break;
                }

                Point3d point = res.Point;

                GraphNode existing = FindNodeNear(point, 5.0, ghostNode.NodeId)!;

                if (existing != null && existing.NodeId != previousNode?.NodeId)
                {
                    ghostNode.DbEntity.Erase();

                    if (ghostEdge != null)
                    {
                        ghostEdge.DbEntity.Erase();
                        if (!EdgeExists(previousNode.NodeId, existing.NodeId))
                        {
                            CreateEdge(previousNode.NodeId, existing.NodeId);
                        }
                    }
                    previousNode = existing;
                }
                else
                {
                    ghostNode.TryModify(1);
                    ghostNode.Position = point;
                    ghostNode.DbEntity.Update();
                    previousNode = ghostNode;
                }
            }
        }

        private bool EdgeExists(Guid nodeA, Guid nodeB)
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphEdge));
            var ids = McObjectManager.SelectObjects(filter);

            foreach (var id in ids)
            {
                if (id.GetObject() is GraphEdge edge
                    && edge.IsIncidentTo(nodeA)
                    && edge.IsIncidentTo(nodeB))
                {
                    return true;
                }
            }

            return false;
        }
    }
}