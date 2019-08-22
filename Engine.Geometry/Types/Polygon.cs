using System.Collections;
using System.Collections.Generic;
using Engine.Core;

namespace Engine.Geometry.Types
{
    public sealed class Polygon : IEnumerable<Vertex>
    {
        private List<Vertex> _points;

        public Polygon(IEnumerable<Vertex> points)
        {
            Verify.IsNotNull(points, nameof(points));
            _points = new List<Vertex>(points);
        }

        public int VertexCount => _points.Count;

        public Vertex this[int index] => _points[index];

        public IEnumerator<Vertex> GetEnumerator()
        {
            return _points.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
