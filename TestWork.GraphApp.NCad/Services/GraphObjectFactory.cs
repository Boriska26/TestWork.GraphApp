using Multicad.Geometry;
using TestWork.GraphApp.NCad.Objects;

namespace TestWork.GraphApp.NCad.Services
{
    public class GraphObjectFactory
    {
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

        public GraphEdge CreateEdge(Guid firstNodeId, Guid secondNodeId)
        {
            var edge = new GraphEdge(firstNodeId, secondNodeId);
            edge.DbObject.AddToCurrentDocument();
            edge.TryModify(1);
            edge.DbObject.Update();

            return edge;
        }
    }
}
