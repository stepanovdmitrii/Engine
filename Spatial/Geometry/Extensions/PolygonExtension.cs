using System;
using Engine.Geometry.Types;

namespace Engine.Geometry.Extensions
{
    public static class PolygonExtension
    {
        public static BoundingBox GetBoundingBox(this Polygon polygon)
        {
            double minX = double.MaxValue;
            double minY = double.MaxValue;
            double maxX = double.MinValue;
            double maxY = double.MinValue;

            foreach(Vertex v in polygon)
            {
                minX = Math.Min(minX, v.X);
                minY = Math.Min(minY, v.Y);
                maxX = Math.Max(maxX, v.X);
                maxY = Math.Max(maxY, v.Y);
            }

            return new BoundingBox(minX, minY, maxX, maxY);
        }
    }
}
