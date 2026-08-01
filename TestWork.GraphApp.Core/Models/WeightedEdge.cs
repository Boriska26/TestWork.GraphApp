namespace TestWork.GraphApp.Core.Models
{
    public readonly struct WeightedEdge
    {
        public readonly Guid First { get; }

        public readonly Guid Second { get; }

        public readonly double Weight { get; }

        public WeightedEdge(Guid first, Guid second, double weight)
        {
            First = first;
            Second = second;
            Weight = weight;
        }
    }
}
