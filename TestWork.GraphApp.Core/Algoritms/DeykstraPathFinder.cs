using TestWork.GraphApp.Core.Models;

namespace TestWork.GraphApp.Core.Algoritms
{
    public class DeykstraPathFinder
    {
        public List<Guid> Find(Graph graph, Guid start, Guid end)
        {
            if (start == end)
            {
                return new() { start};
            }

            var distances = new Dictionary<Guid, double>();
            var previous = new Dictionary<Guid, Guid>();
            var unvisited = new HashSet<Guid>();

            foreach (var node in graph.Nodes)
            {
                distances[node] = double.PositiveInfinity;
                unvisited.Add(node);
            }

            if (!distances.ContainsKey(start) || !distances.ContainsKey(end))
            {
                return new();
            }

            distances[start] = 0;

            while (unvisited.Count > 0)
            {
                Guid current = unvisited.OrderBy(n => distances[n]).First();
                if (distances[current] == double.PositiveInfinity)
                {
                    break;
                }

                if (current == end)
                {
                    return ReconstructPath(previous, start, end);
                }

                unvisited.Remove(current);

                foreach (var neighbor in graph.GetNeighbors(current))
                {
                    if (!unvisited.Contains(neighbor.NodeId))
                    {
                        continue;
                    }

                    double alt = distances[current] + neighbor.Weight;
                    if (alt < distances[neighbor.NodeId])
                    {
                        distances[neighbor.NodeId] = alt;
                        previous[neighbor.NodeId] = current;
                    }
                }
            }

            return new();
        }

        private List<Guid> ReconstructPath(Dictionary<Guid, Guid> previous, Guid start, Guid goal)
        {
            var path = new List<Guid>();
            Guid current = goal;

            while (current != start)
            {
                path.Add(current);
                if (!previous.TryGetValue(current, out current))
                {
                    return new();
                }
            }
            path.Add(start);
            path.Reverse();
            {
                return path;
            }
        }
    }
}
