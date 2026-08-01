using Multicad;
using Multicad.DatabaseServices;
using Multicad.Geometry;
using System;
using TestWork.GraphApp.NCad.Extensions;
using TestWork.GraphApp.NCad.Objects;

namespace TestWork.GraphApp.NCad.Services
{
    public class GraphBuilder
    {
        private readonly GraphQuery _query;
        private readonly GraphObjectFactory _factory;

        public GraphBuilder(GraphQuery query, GraphObjectFactory factory)
        {
            _query = query;
            _factory = factory;
        }

        public void Build(NodeShape shape)
        {
            GraphNode previousNode = null;

            while (true)
            {
                Point3d startPos = previousNode?.Position ?? Point3d.Origin;
                GraphNode ghostNode = _factory.CreateNode(startPos, shape);

                GraphEdge ghostEdge = null;
                if (previousNode != null)
                {
                    ghostEdge = _factory.CreateEdge(previousNode.NodeId, ghostNode.NodeId);
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

                GraphNode existing = _query.FindNodeNear(point, GraphNode.Radius, ghostNode.NodeId);
                GraphEdge edgeToSplit = _query.FindEdgeNear(point, GraphNode.Radius, ghostEdge);

                if (existing != null && existing.NodeId != previousNode?.NodeId)
                {
                    ghostNode.DbEntity.Erase();

                    if (ghostEdge != null)
                    {
                        ghostEdge.DbEntity.Erase();
                        if (!_query.EdgeExists(previousNode.NodeId, existing.NodeId))
                        {
                            _factory.CreateEdge(previousNode.NodeId, existing.NodeId);
                        }
                    }
                    previousNode = existing;
                }
                else if (edgeToSplit != null)
                {
                    Guid splitA = edgeToSplit.FirstNodeId;
                    Guid splitB = edgeToSplit.SecondNodeId;

                    _query.TryGetNodePosition(splitA, out Point3d pA);
                    _query.TryGetNodePosition(splitB, out Point3d pB);

                    Point3d onEdge = point.ToCorePoint()
                        .ProjectOnSegment(pA.ToCorePoint(), pB.ToCorePoint())
                        .ToMulticadPoint();

                    ghostNode.TryModify(1);
                    ghostNode.Position = onEdge;
                    ghostNode.DbEntity.Update();

                    edgeToSplit.DbEntity.Erase();
                    _factory.CreateEdge(splitA, ghostNode.NodeId);
                    _factory.CreateEdge(ghostNode.NodeId, splitB);

                    previousNode = ghostNode;
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
    }
}