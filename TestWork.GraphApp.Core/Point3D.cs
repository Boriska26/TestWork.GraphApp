using System.Diagnostics.CodeAnalysis;

namespace TestWork.GraphApp.Core
{
    /// <summary>
    /// Точка
    /// </summary>
    public readonly struct Point3D
    {
        public Point3D(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

        public double X { get; }

        public double Y { get; }

        public double Z { get; }

        public double Distance(Point3D other)
        {
            double dx = X - other.X;
            double dy = Y - other.Y;
            double dz = Z - other.Z;

            return Math.Sqrt(dx * dx + dy * dy + dz * dz);
        }

        public bool Equals(Point3D other) => X == other.X && Y == other.Y && Z == other.Z;

        public override bool Equals([NotNullWhen(true)] object? obj)
        {
            return obj is Point3D oher && Equals(obj);
        }

        public override int GetHashCode() => HashCode.Combine(X, Y, Z);
    }
}