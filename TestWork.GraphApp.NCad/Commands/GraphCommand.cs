using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Multicad.DatabaseServices;
using Multicad.Runtime;
using TestWork.GraphApp.NCad.Objects;
using MGeo = Multicad.Geometry;
using TGeo = Teigha.Geometry;

namespace TestWork.GraphApp.NCad.Commands
{
    public class GraphCommand
    {
        [CommandMethod("GRAPHNODE", CommandFlags.NoCheck)]
        public void CreateNode()
        {
            Document doc = Application.DocumentManager.MdiActiveDocument;
            if (doc == null)
                return;

            Editor ed = doc.Editor;

            PromptPointResult res = ed.GetPoint("\nУкажите положение узла: ");
            if (res.Status != PromptStatus.OK)
                return;

            TGeo.Point3d tp = res.Value;
            var pos = new MGeo.Point3d(tp.X, tp.Y, tp.Z);

            var node = new GraphNode
            {
                Position = pos,
                Shape = NodeShape.CircleBlue
            };

            node.DbEntity.AddToCurrentDocument();
        }

        [CommandMethod("GRAPHEDGE_TEST", CommandFlags.NoCheck)]
        public void TestEdge()
        {
            var filter = ObjectFilter.Create(true);
            filter.AddType(typeof(GraphNode));
            var ids = McObjectManager.SelectObjects(filter);

            if (ids.Count < 2) return;

            var n1 = ids[0].GetObject() as GraphNode;
            var n2 = ids[1].GetObject() as GraphNode;

            var edge = new GraphEdge(n1.NodeId, n2.NodeId);
            edge.DbEntity.AddToCurrentDocument();
        }
    }
}