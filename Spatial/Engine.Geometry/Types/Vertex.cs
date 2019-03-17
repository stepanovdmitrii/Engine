namespace Engine.Geometry.Types
{
    public struct Vertex
    {
        public double X { get; private set; }
        public double Y { get; private set; }

        public Vertex(double x, double y)
        {
            X = x;
            Y = y;
        }

        public static Vertex operator /(Vertex v, double value)
        {
            return new Vertex(v.X / value, v.Y / value);
        }
    }
}
