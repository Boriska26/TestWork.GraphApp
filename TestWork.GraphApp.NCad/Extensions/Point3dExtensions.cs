using Multicad.Geometry;
using TestWork.GraphApp.Core;

namespace TestWork.GraphApp.NCad.Extensions
{
    public static class Point3dExtensions
    {
        public static Point3D ToCorePoint(this Point3d point) => new Point3D(point.X, point.Y, point.Z);

        public static Point3d ToMulticadPoint(this Point3D point) => new Point3d(point.X, point.Y, point.Z);
    }
}
