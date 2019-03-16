using System;

namespace Engine.Geometry.Types
{
    public struct Vector2D
    {
        public Vector2D(double x, double y)
        {
            X = x;
            Y = y;
        }

        public Vector2D(Vertex v)
        {
            X = v.X;
            Y = v.Y;
        }

        public double X { get; private set; }
        public double Y { get; private set; }

        public double Abs()
        {
            return Math.Sqrt(X * X + Y * Y);
        }
        public static Vector2D operator + (Vector2D first, Vector2D second)
        {
            return new Vector2D(first.X + second.X, first.Y + second.Y);
        }
        public static Vector2D operator - (Vector2D first, Vector2D second)
        {
            return new Vector2D(first.X - second.X, first.Y - second.Y);
        }
        public static Vector2D operator * (Vector2D vector, double value)
        {
            return new Vector2D(vector.X * value, vector.Y * value);
        }
        public static Vector2D operator * (double value, Vector2D vector)
        {
            return vector * value;
        }
        public static double operator * (Vector2D first, Vector2D second)
        {
            return first.X * second.X + first.Y * second.Y;
        }
        public static Vector2D operator / (Vector2D vector, double value)
        {
            return vector * (1 / value);
        }
        public static double operator & (Vector2D first, Vector2D second)
        {
            return first.X * second.Y - second.X * first.Y;
        }
    }
}
