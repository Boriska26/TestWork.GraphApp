namespace TestWork.GraphApp.Core.Models
{
    public class Graph
    {
        private readonly HashSet<Guid> _nodes = new();
        private readonly List<WeightedEdge> _edges = new();

        public void AddNode(Guid nodeId) => _nodes.Add(nodeId);

        public void AddEdge(Guid firstNodeId, Guid secondNodeId, double weight)
        {
            _nodes.Add(firstNodeId);
            _nodes.Add(secondNodeId);
            _edges.Add(new WeightedEdge(firstNodeId, secondNodeId, weight));
        }

        public IReadOnlyCollection<Guid> Nodes => _nodes;

        public IEnumerable<Neighbor> GetNeighbors(Guid nodeId)
        {
            foreach (var edge in _edges)
            {
                if (edge.First == nodeId)
                {
                    yield return new Neighbor(edge.Second, edge.Weight);
                }
                else if (edge.Second == nodeId)
                {
                    yield return new Neighbor(edge.First, edge.Weight);
                }
            }
        }
    }
}
