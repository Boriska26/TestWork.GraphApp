using Multicad.Geometry;
using TestWork.GraphApp.NCad.Objects;

namespace TestWork.GraphApp.NCad.Services
{
    public class GraphObjectFactory
    {
        private readonly GraphQuery _graphQuery;

        public GraphObjectFactory(GraphQuery query)
        {
            _graphQuery = query;
        }

        public GraphNode CreateNode(Point3d position, NodeShape shape)
        {
            var node = new GraphNode()
            {
                Position = position,
                Shape = shape
            };
            node.DbEntity.AddToCurrentDocument();

            return node;
        }

        public GraphEdge CreateEdge(Guid firstNode, Guid secondNode)
        {
            var edge = new GraphEdge(firstNode, secondNode);
            edge.DbObject.AddToCurrentDocument();

            if (_graphQuery.TryGetNodePosition(firstNode, out Point3d start) &&
                _graphQuery.TryGetNodePosition(secondNode, out Point3d end))
            {
                edge.UpdatePositions(start, end);
            }

            return edge;
        }
    }
}
