namespace Engine.Geometry.Types
{
    public struct LineSegment
    {
        public LineSegment(Vertex start, Vertex end)
        {
            Start = start;
            End = end;
        }

        public Vertex Start { get; }

        public Vertex End { get; }
    }
}
