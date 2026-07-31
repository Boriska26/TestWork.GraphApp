namespace TestWork.GraphApp.Core.Models
{
    public class Node
    {
        public Guid Id { get; }

        public Point3D Positions { get; set; }

        public Node(Point3D position)
        {
            Id = Guid.NewGuid();
            Positions = position;
        }
    }
}
