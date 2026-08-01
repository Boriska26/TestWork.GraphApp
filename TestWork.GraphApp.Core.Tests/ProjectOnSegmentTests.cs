namespace TestWork.GraphApp.Core.Tests
{
    public class ProjectOnSegmentTests
    {
        private const int Precision = 9;

        [Fact]
        public void Midpoint_ProjectsToMiddle()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(10, 0, 0);
            var p = new Point3D(5, 5, 0);

            var proj = p.ProjectOnSegment(a, b);

            Assert.Equal(5, proj.X, Precision);
            Assert.Equal(0, proj.Y, Precision);
            Assert.Equal(0, proj.Z, Precision);
        }

        [Fact]
        public void PointBeyondEnd_ClampsToEndpoint()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(10, 0, 0);
            var p = new Point3D(20, 5, 0);

            var proj = p.ProjectOnSegment(a, b);

            Assert.Equal(10, proj.X, Precision);
            Assert.Equal(0, proj.Y, Precision);
        }

        [Fact]
        public void PointBeforeStart_ClampsToStart()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(10, 0, 0);
            var p = new Point3D(-5, 3, 0);

            var proj = p.ProjectOnSegment(a, b);

            Assert.Equal(0, proj.X, Precision);
            Assert.Equal(0, proj.Y, Precision);
        }

        [Fact]
        public void PointOnSegment_ReturnsItself()
        {
            var a = new Point3D(0, 0, 0);
            var b = new Point3D(10, 0, 0);
            var p = new Point3D(3, 0, 0);

            var proj = p.ProjectOnSegment(a, b);

            Assert.Equal(3, proj.X, Precision);
            Assert.Equal(0, proj.Y, Precision);
        }

        [Fact]
        public void DegenerateSegment_ReturnsEndpoint()
        {
            var a = new Point3D(5, 5, 5);
            var b = new Point3D(5, 5, 5);
            var p = new Point3D(10, 10, 10);

            var proj = p.ProjectOnSegment(a, b);

            Assert.Equal(5, proj.X, Precision);
            Assert.Equal(5, proj.Y, Precision);
            Assert.Equal(5, proj.Z, Precision);
        }
    }
}