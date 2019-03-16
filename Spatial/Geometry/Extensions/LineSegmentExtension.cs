using Engine.Core;
using Engine.Geometry.Types;

namespace Engine.Geometry.Extensions
{
    public static class LinesegmentExtension
    {
        public static Vertex GetVertexAtParameter(this LineSegment lineSegment, double t)
        {
            Verify.InRangeInclude(t, 0, 1, nameof(t));

            double newX = lineSegment.Start.X + t * (lineSegment.End.X - lineSegment.Start.X);
            double newY = lineSegment.Start.Y + t * (lineSegment.End.Y - lineSegment.Start.Y);

            return new Vertex(newX, newY);
        }
    }
}
