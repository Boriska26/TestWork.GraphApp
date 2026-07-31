using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Drawing;

namespace TestWork.GraphApp.NCad.Objects
{
    [CustomEntity("D7983907-8A95-4A2A-A418-4BE05A8082FB", "GraphEdge", "ребро графа")]
    public class GraphEdge : McCustomBase, IMcSerializable
    {
        private Guid _firstNodeId;
        private Guid _secondNodeId;

        public GraphEdge() { }

        public GraphEdge(Guid firstNodeId, Guid secondNodeId)
        {
            _firstNodeId = firstNodeId;
            _secondNodeId = secondNodeId;
        }

        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();

            if (TryGetNodePosition(_firstNodeId, out Point3d p1) &&
                TryGetNodePosition(_secondNodeId, out Point3d p2))
            {
                dc.Color = McDbEntity.ByObject;
                dc.DrawPolyline(new[] { p1, p2 });
            }
        }

        private bool TryGetNodePosition(Guid nodeId, out Point3d position)
        {
            position = Point3d.Origin;
            foreach (var node in EnumerateGraphNodes())
            {
                if (node.NodeId == nodeId)
                {
                    position = node.Position;

                    return true;
                }
            }

            return false;
        }

        private IEnumerable<GraphNode> EnumerateGraphNodes()
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphNode));
            List<McObjectId> ids = McObjectManager.SelectObjects(filter);
            foreach (var id in ids)
            {
                if (id.GetObject() is GraphNode node)
                {
                    yield return node;
                }
            }
        }

        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("FirstNodeId", _firstNodeId.ToString());
            info.Add("SecondNodeId", _secondNodeId.ToString());

            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            string firstNodeId;
            string secondNodeId;

            info.GetValue("FirstNodeId", out firstNodeId);
            info.GetValue("SecondNodeId", out secondNodeId);

            _firstNodeId = Guid.Parse(firstNodeId);
            _secondNodeId = Guid.Parse(secondNodeId);

            return hresult.s_Ok;
        }

        public bool IsIncidentTo(Guid nodeId) => _firstNodeId == nodeId || _secondNodeId == nodeId;
    }
}