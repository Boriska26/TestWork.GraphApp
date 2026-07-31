namespace TestWork.GraphApp.Core.Tests
{
    public class Point3DTests
    {
        private const int _precision = 9;

        [Fact]
        public void Distance_ToSamePoint_IsZero()
        {
            var p = new Point3D(1, 2, 3);
            Assert.Equal(0, p.Distance(p),_precision);
        }

        [Fact]
        public void Distance_AlongXAxis_EqualsCoordinateDifference()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(5, 0, 0);
            Assert.Equal(5, a.Distance(b), _precision);
        }

        [Fact]
        public void Distance_Diagonal2D_Is5_For3And4()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(3, 4, 0);
            Assert.Equal(5, a.Distance(b), _precision);
        }

        [Fact]
        public void Distance_Full3D_IsCorrect()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(2, 3, 6);
            Assert.Equal(7, a.Distance(b), _precision);
        }

        [Fact]
        public void Distance_IsSymmetric()
        {
            var a = new Point3D(1, 2, 3);
            var b = new Point3D(4, 6, 8);
            Assert.Equal(a.Distance(b), b.Distance(a), _precision);
        }

        [Fact]
        public void Distance_HandlesNegativeCoordinates()
        {
            var a = new Point3D(-1, -1, -1);
            var b = new Point3D(2, 3, -1);
            Assert.Equal(5, a.Distance(b), _precision);
        }

        [Theory]
        [InlineData(0, 0, 0, 1, 0, 0, 1)]
        [InlineData(0, 0, 0, 0, 0, 10, 10)]
        [InlineData(1, 1, 1, 1, 1, 1, 0)]
        public void Distance_Parametrized(
            double x1, double y1, double z1,
            double x2, double y2, double z2,
            double expected)
        {
            var a = new Point3D(x1, y1, z1);
            var b = new Point3D(x2, y2, z2);
            Assert.Equal(expected, a.Distance(b), _precision);
        }
    }
}
