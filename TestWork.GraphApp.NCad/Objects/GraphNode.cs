using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;
using System.ComponentModel;
using System.Drawing;

namespace TestWork.GraphApp.NCad.Objects
{
    [CustomEntity("C8D12A1F-3B9B-4D3E-91A2-8F0123456789", "GraphNode", "узел графа")]
    public class GraphNode : McCustomBase, IMcSerializable
    {
        public const double Radius = 5.0;

        private Guid _nodeId = Guid.NewGuid();
        private Point3d _position = Point3d.Origin;
        private NodeShape _shape = NodeShape.CircleBlue;

        public Guid NodeId => _nodeId;

        public Point3d Position
        {
            get => _position;
            set
            {
                _position = value;
                DbEntity.Update();
            }
        }

        [DisplayName("Форма узла")]
        [Description("Тип узла: cний круг или красный треугольник")]
        [Category("Граф")]
        //TODO в палитре не показывается имя shape
        public NodeShape Shape
        {
            get => _shape;
            set
            {
                if(!TryModify())
                {
                    return;
                }
                _shape = value;
            }
        }

        private bool TryModify()
        {
            TryModify(0);

            return true;
        }

        public override bool GetGripPoints(GripPointsInfo info)
        {
            info.AppendGrip(new McSmartGrip<GraphNode>(_position, (obj, grip, offset) =>
            {
                obj.TryModify();
                obj._position += offset;
                obj.UpdateIncidentEdges();
            }));

            return true;
        }

        public override void OnTransform(Matrix3d tfm)
        {
            TryModify(1);
            _position = _position.TransformBy(tfm);
            UpdateIncidentEdges();
        }

        public override void OnDraw(GeometryBuilder dc)
        {
            dc.Clear();
            switch (_shape)
            {
                case NodeShape.CircleBlue:
                    DrawCircleBlue(dc);
                    break;
                case NodeShape.TriangleRed:
                    DrawTriangleRed(dc);
                    break;
            }
        }

        private void DrawCircleBlue(GeometryBuilder geometryBuilder)
        {
            geometryBuilder.Color = Color.Blue;
            geometryBuilder.DrawCircle(_position, Radius);
        }

        private void DrawTriangleRed(GeometryBuilder geometryBuilder)
        {
            geometryBuilder.Color = Color.Red;
            var p1 = new Point3d(_position.X, _position.Y + Radius, _position.Z);
            var p2 = new Point3d(_position.X - Radius, _position.Y - Radius * 0.6, _position.Z);
            var p3 = new Point3d(_position.X + Radius, _position.Y - Radius * 0.6, _position.Z);

            geometryBuilder.DrawPolyline(new[] { p1, p2, p3, p1 });
        }

        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("NodeId", _nodeId.ToString());
            info.Add("Shape", (int)_shape);
            info.Add("PosX", _position.X);
            info.Add("PosY", _position.Y);
            info.Add("PosZ", _position.Z);
            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            string nodeId;
            int shape;
            double x;
            double y;
            double z;

            info.GetValue("NodeId", out nodeId);
            info.GetValue("Shape", out shape);
            info.GetValue("PosX", out x);
            info.GetValue("PosY", out y);
            info.GetValue("PosZ", out z);

            _nodeId = Guid.Parse(nodeId);
            _shape = (NodeShape)shape;
            _position = new Point3d(x, y, z);

            return hresult.s_Ok;
        }

        private void UpdateIncidentEdges()
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphEdge));
            var ids = McObjectManager.SelectObjects(filter); 
            foreach(var id in ids)
            {
                if(id.GetObject() is GraphEdge edge && edge.IsIncidentTo(_nodeId))
                {
                    edge.TryModify(1);
                    edge.DbEntity.Update();
                }
            }
        }

        public override void OnErase()
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphEdge));
            var ids = McObjectManager.SelectObjects(filter);
            foreach (var id in ids)
            {
                if (id.GetObject() is GraphEdge edge && edge.IsIncidentTo(_nodeId))
                {
                    edge.DbEntity.Erase();
                }
            }
        }
    }
}