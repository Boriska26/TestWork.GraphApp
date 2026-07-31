using Multicad;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.Drawing;
using TestWork.GraphApp.NCad.Objects;
using GraphNode = TestWork.GraphApp.NCad.Objects.GraphNode;

namespace TestWork.GraphApp.NCad.Commands
{
    public class GraphCommand
    {
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

        //TODO рисовать призрака перед вставкой
        [CommandMethod("TW_BUILDGRAPH", CommandFlags.NoCheck)]
        public void BuildGraph()
        {
            GraphNode previousNode = null;

            while (true)
            {
                var inputJig = new InputJig();
                InputResult res = inputJig.GetPoint("Выберите точку для узла: ");
                if (res.Result != InputResult.ResultCode.Normal)
                {
                    break;
                }

                Point3d point = res.Point;

                GraphNode currentNode = FindNodeNear(point, 5.0) ?? CreateNode(point);

                if (previousNode != null && previousNode.NodeId != currentNode.NodeId)
                {
                    CreateEdge(previousNode.NodeId, currentNode.NodeId);
                }

                previousNode = currentNode;
            }
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

        private GraphNode? FindNodeNear(Point3d point, double captureRadius)
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphNode));
            var ids = McObjectManager.SelectObjects(filter);

            foreach (var id in ids)
            {
                if (id.GetObject() is GraphNode node && node.Position.DistanceTo(point) <= captureRadius)
                {
                    return node;
                }
            }

            return null;
        }
    }
}