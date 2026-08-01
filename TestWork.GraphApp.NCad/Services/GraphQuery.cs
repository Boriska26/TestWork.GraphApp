using Multicad.DatabaseServices;
using Multicad.Geometry;
using TestWork.GraphApp.NCad.Extensions;
using TestWork.GraphApp.NCad.Objects;

namespace TestWork.GraphApp.NCad.Services
{
    public class GraphQuery
    {
        public IEnumerable<GraphNode> EnumerateNodes()
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphNode));
            foreach (var id in McObjectManager.SelectObjects(filter))
            {
                if (id.GetObject() is GraphNode node)
                {
                    yield return node;
                }
            }
        }

        public IEnumerable<GraphEdge> EnumerateEdges()
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphEdge));
            foreach (var id in McObjectManager.SelectObjects(filter))
            {
                if (id.GetObject() is GraphEdge edge)
                {
                    yield return edge;
                }
            }
        }

        public GraphNode FindNodeNear(Point3d point, double captureRadius, Guid excludeNodeId)
        {
            foreach (var node in EnumerateNodes())
            {
                if (node.NodeId != excludeNodeId
                    && node.Position.DistanceTo(point) <= captureRadius)
                {
                    return node;
                }
            }

            return null;
        }

        public GraphEdge FindEdgeNear(Point3d point, double captureDistance, GraphEdge excludeEdge)
        {
            foreach (var edge in EnumerateEdges())
            {
                if (excludeEdge != null
                    && edge.FirstNodeId == excludeEdge.FirstNodeId
                    && edge.SecondNodeId == excludeEdge.SecondNodeId)
                {
                    continue;
                }

                if (TryGetNodePosition(edge.FirstNodeId, out Point3d pA)
                    && TryGetNodePosition(edge.SecondNodeId, out Point3d pB))
                {
                    Point3d projection = point.ToCorePoint()
                        .ProjectOnSegment(pA.ToCorePoint(), pB.ToCorePoint())
                        .ToMulticadPoint();

                    if (point.DistanceTo(projection) <= captureDistance)
                    {
                        return edge;
                    }
                }
            }

            return null;
        }

        public bool TryGetNodePosition(Guid nodeId, out Point3d position)
        {
            position = Point3d.Origin;
            foreach (var node in EnumerateNodes())
            {
                if (node.NodeId == nodeId)
                {
                    position = node.Position;

                    return true;
                }
            }

            return false;
        }

        public bool EdgeExists(Guid nodeA, Guid nodeB)
        {
            foreach (var edge in EnumerateEdges())
            {
                if (edge.IsIncidentTo(nodeA) && edge.IsIncidentTo(nodeB))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
