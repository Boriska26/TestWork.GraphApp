using TestWork.GraphApp.Core.Exceptions;

namespace TestWork.GraphApp.Core.Models
{
    public class Edge
    {
        public Edge(Guid firstNodeId, Guid secondNodeId)
        {
            if (firstNodeId == secondNodeId)
            {
                throw new EdgeException("Ребро не может иметь два одинаковых узла");
            }

            Id = Guid.NewGuid();
            FirstNodeId = firstNodeId;
            SecondNodeId = secondNodeId;
        }

        public Guid Id { get; }

        public Guid FirstNodeId { get; }

        public Guid SecondNodeId { get; }

        public bool IsIncidentTo(Guid nodeId) => FirstNodeId == nodeId || SecondNodeId == nodeId;

        public Guid GetOtherNode(Guid nodeId)
        {
            if(FirstNodeId == nodeId)
            {
                return SecondNodeId;
            }
            if (SecondNodeId == nodeId)
            {
                return FirstNodeId;
            }

            throw new EdgeException("Узел не принадлежит этому ребру");
        }
    }
}
