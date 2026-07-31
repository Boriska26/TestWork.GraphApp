using System;
using HostMgd.ApplicationServices;
using HostMgd.EditorInput;
using Multicad.Runtime;
using MGeo = Multicad.Geometry;
using TGeo = Teigha.Geometry;
using TestWork.GraphApp.NCad.Objects;

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

            // 1. Запросить точку у пользователя
            PromptPointResult res = ed.GetPoint("\nУкажите положение узла: ");
            if (res.Status != PromptStatus.OK)
                return;

            TGeo.Point3d tp = res.Value;                       // Teigha-точка
            var pos = new MGeo.Point3d(tp.X, tp.Y, tp.Z);      // -> MultiCAD-точка

            // 2. Создать узел
            var node = new GraphNode
            {
                Position = pos,
                Shape = NodeShape.CircleBlue
            };

            // 3. Добавить в чертёж — ЭТУ СТРОКУ СВЕРЬ ПО СВОЕЙ dll (см. ниже)
            node.DbEntity.AddToCurrentDocument();
        }
    }
}