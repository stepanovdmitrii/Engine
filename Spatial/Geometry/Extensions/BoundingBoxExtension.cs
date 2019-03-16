using Engine.Geometry.Types;

namespace Engine.Geometry.Extensions
{
    public static class BoundingBoxExtension
    {
        public static double GetDeltaX(this BoundingBox boundingBox) => boundingBox.MaxX - boundingBox.MinX;
        public static double GetDeltaY(this BoundingBox boundingBox) => boundingBox.MaxY - boundingBox.MinY;
    }
}
