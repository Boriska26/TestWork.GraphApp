using Imapimgd.Interfaces;
using Multicad;
using Multicad.CustomObjectBase;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using Multicad.Runtime;

namespace TestWork.GraphApp.NCad.Objects
{
    [CustomEntity("D7983907-8A95-4A2A-A418-4BE05A8082FB", "GraphEdge", "ребро графа")]
    public class GraphEdge : McCustomBase, IMcSerializable, IMcParameterRedefinitions
    {
        private Guid _firstNodeId;
        private Guid _secondNodeId;
        private Point3d _startPoint;
        private Point3d _endPoint;

        public GraphEdge() : base() { }

        public GraphEdge(Guid firstNodeId, Guid secondNodeId)
        {
            _firstNodeId = firstNodeId;
            _secondNodeId = secondNodeId;
        }

        public Guid FirstNodeId => _firstNodeId;

        public Guid SecondNodeId => _secondNodeId;

        public void UpdatePositions(Point3d start, Point3d end)
        {
            TryModify(1);
            _startPoint = start;
            _endPoint = end;
            DbEntity.Update();
        }

        public override string GetProperyNameByGSMarker(int iGSMarker)
        {
            return base.GetProperyNameByGSMarker(iGSMarker);
        }

        public override bool OnMatchProperties(McEntity EntFrom, MatchPropEnum matchPropFlags)
        {
            return base.OnMatchProperties(EntFrom, matchPropFlags);
        }

        public override void TryModify(uint dwChangesType)
        {
            base.TryModify(dwChangesType);
        }

        public override void OnDraw(GeometryBuilder dc)
        {
            base.OnDraw(dc);
            dc.Clear();

            dc.Color = McDbEntity.ByObject;
            dc.LineWidth = DbEntity.LineWeight;
            dc.LineType = DbEntity.LineType;
            dc.StrLineType = DbEntity.LineTypeName;
            dc.DrawLine(_startPoint, _endPoint);
        }

        public override UpdateLevel OnQueryUpdateLevel()
        {
            return base.OnQueryUpdateLevel();
        }

        public override hresult OnUpdate()
        {
            var result = base.OnUpdate();
            return result;
        }

        public override hresult OnMcSerialization(McSerializationInfo info)
        {
            info.Add("FirstNodeId", _firstNodeId.ToString());
            info.Add("SecondNodeId", _secondNodeId.ToString());
            info.Add("StartX", _startPoint.X);
            info.Add("StartY", _startPoint.Y);
            info.Add("StartZ", _startPoint.Z);
            info.Add("EndX", _endPoint.X);
            info.Add("EndY", _endPoint.Y);
            info.Add("EndZ", _endPoint.Z);

            return hresult.s_Ok;
        }

        public override hresult OnMcDeserialization(McSerializationInfo info)
        {
            string firstNodeId;
            string secondNodeId;
            double startX;
            double startY;
            double startZ;
            double endX;
            double endY;
            double endZ;
            info.GetValue("StartX", out startX);
            info.GetValue("StartY", out startY);
            info.GetValue("StartZ", out startZ);
            info.GetValue("EndX", out endX);
            info.GetValue("EndY", out endY);
            info.GetValue("EndZ", out endZ);
            info.GetValue("FirstNodeId", out firstNodeId);
            info.GetValue("SecondNodeId", out secondNodeId);

            _startPoint = new Point3d(startX, startY, startZ);
            _endPoint = new Point3d(endX, endY, endZ);
            _firstNodeId = Guid.Parse(firstNodeId);
            _secondNodeId = Guid.Parse(secondNodeId);

            return hresult.s_Ok;
        }

        public bool IsIncidentTo(Guid nodeId) => _firstNodeId == nodeId || _secondNodeId == nodeId;

        public List<int> GetRedefinitions()
        {
            throw new NotImplementedException();
        }
    }
}