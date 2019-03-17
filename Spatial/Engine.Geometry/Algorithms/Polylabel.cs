using System;
using System.Collections.Generic;
using System.Linq;
using Engine.Core;
using Engine.Geometry.Extensions;
using Engine.Geometry.Types;

namespace Engine.Geometry.Algorithms
{
    public static class Polylabel
    {
        public static Vertex ComputeLabelPosition(Polygon polygon, double eps = 1E-6)
        {
            Verify.IsNotNull(polygon, nameof(polygon));
            Verify.InRangeInclude(polygon.VertexCount, 3, int.MaxValue, nameof(polygon.VertexCount));
            Verify.InRangeExclude(eps, 0, double.PositiveInfinity, nameof(eps));

            BoundingBox bbox = polygon.GetBoundingBox();
            double cellSize = Math.Min(bbox.GetDeltaX(), bbox.GetDeltaY());
            if (IsZero(cellSize))
            {
                return new Vertex(bbox.MinX, bbox.MinY);
            }

            double halfSize = cellSize / 2;

            SortedSet<Cell> queue = InitializeStartGrid(polygon, ref bbox, cellSize, halfSize);

            Cell bestCell = InitializeStartCell(polygon, ref bbox);

            while (queue.Any())
            {
                Cell probe = queue.Max;
                queue.Remove(queue.Max);
                if (probe.DistanceToPolygon > bestCell.DistanceToPolygon)
                {
                    bestCell = probe;
                }
                if (probe.MaxDistance - bestCell.DistanceToPolygon < eps)
                {
                    continue;
                }

                double newHalfSize = probe.HalfSize / 2.0;
                queue.Add(CreateCell(probe.Center.X - newHalfSize, probe.Center.Y - newHalfSize, newHalfSize, polygon));
                queue.Add(CreateCell(probe.Center.X + newHalfSize, probe.Center.Y - newHalfSize, newHalfSize, polygon));
                queue.Add(CreateCell(probe.Center.X - newHalfSize, probe.Center.Y + newHalfSize, newHalfSize, polygon));
                queue.Add(CreateCell(probe.Center.X + newHalfSize, probe.Center.Y + newHalfSize, newHalfSize, polygon));
            }
            return bestCell.Center;

        }

        private static SortedSet<Cell> InitializeStartGrid(Polygon polygon, ref BoundingBox bbox, double cellSize, double halfSize)
        {
            var queue = new SortedSet<Cell>(new CellComparer());
            for (double x = bbox.MinX; x < bbox.MaxX; x += cellSize)
            {
                for (double y = bbox.MinY; y < bbox.MaxY; y += cellSize)
                {
                    queue.Add(CreateCell(x + halfSize, y + halfSize, halfSize, polygon));
                }
            }

            return queue;
        }

        private static Cell InitializeStartCell(Polygon polygon, ref BoundingBox bbox)
        {
            Cell bestCell = ComputeCentroidCell(polygon);

            double rectangeCenterX = bbox.MinX + bbox.GetDeltaX() / 2;
            double rectangleCenterY = bbox.MinY + bbox.GetDeltaY() / 2;
            Cell rectangleCell = CreateCell(rectangeCenterX, rectangleCenterY, 0, polygon);

            if (rectangleCell.DistanceToPolygon > bestCell.DistanceToPolygon)
            {
                bestCell = rectangleCell;
            }

            return bestCell;
        }

        private static Cell ComputeCentroidCell(Polygon polygon)
        {
            double centerX = default(double);
            double centerY = default(double);
            double area = default(double);
            for (int i = 0, len = polygon.VertexCount, j = len - 1; i < len; j = i++)
            {
                Vertex start = polygon[j];
                Vertex end = polygon[i];
                
                double f = end.X * start.Y - end.Y * start.X;
                centerX += (end.X + start.X) * f;
                centerY += (end.Y + start.Y) * f;
                area += f * 3;
            }
            Vertex computedCenter = new Vertex(centerX, centerY);
            Vertex actualCenter = IsZero(area)
                ? polygon[0]
                : computedCenter / area;
            double distanceToPolygon = ComputePointToPolygonDistance(ref actualCenter, polygon);

            if (IsZero(area))
            {
                centerX = polygon[0].X;
                centerY = polygon[0].Y;
            }
            else
            {
                centerX = centerX / area;
                centerY = centerY / area;
            }
            return CreateCell(centerX, centerY, 0, polygon);
        }

        private static Cell CreateCell(double centerX, double centerY, double halfSize, Polygon polygon)
        {
            var center = new Vertex(centerX, centerY);
            double distanceToPolygon = ComputePointToPolygonDistance(ref center, polygon);
            return new Cell(center, halfSize, distanceToPolygon);
        }

        private static double ComputePointToPolygonDistance(ref Vertex point, Polygon polygon)
        {
            bool inside = false;
            double minDist = double.PositiveInfinity;
            for (int i = 0, len = polygon.VertexCount, j = len - 1; i < len; j = i++)
            {
                LineSegment currentEdge = new LineSegment(polygon[j], polygon[i]);

                if (HorizontalRayIntersectsEdge(ref point, ref currentEdge))
                {
                    inside = !inside;
                }
                minDist = Math.Min(minDist, ComputePointToSegmentDistance(ref point, ref currentEdge));
            }
            return (inside ? 1 : -1) * minDist;
        }

        private static bool HorizontalRayIntersectsEdge(ref Vertex point, ref LineSegment edge)
        {
            return (edge.End.Y > point.Y != edge.Start.Y > point.Y)
                && (point.X < (edge.Start.X - edge.End.X) * (point.Y - edge.End.Y) / (edge.Start.Y - edge.End.Y) + edge.End.X);
        }

        private static double ComputePointToSegmentDistance(ref Vertex p, ref LineSegment s)
        {
            double x = s.Start.X;
            double y = s.Start.Y;
            double dx = s.End.X - s.Start.X;
            double dy = s.End.Y - s.Start.Y;
            if (!IsZero(dx) || !IsZero(dy))
            {
                double t = ((p.X - s.Start.X) * dx + (p.Y - s.Start.Y) * dy) / (dx * dx + dy * dy);
                if (t > 1)
                {
                    x = s.End.X;
                    y = s.End.Y;
                }
                else
                {
                    if (t > 0)
                    {
                        x += dx * t;
                        y += dy * t;
                    }
                }
            }
            dx = p.X - x;
            dy = p.Y - y;
            return Math.Sqrt(dx * dx + dy * dy);
        }

        private static bool IsZero(double v)
        {
            return Math.Abs(v) <= double.Epsilon;
        }

        private struct Cell
        {
            public Cell(Vertex center, double halfSize, double distanceToPolygon)
            {
                Center = center;
                HalfSize = halfSize;
                DistanceToPolygon = distanceToPolygon;
                MaxDistance = distanceToPolygon + halfSize * Math.Sqrt(2);
            }

            public double MaxDistance { get; }
            public double DistanceToPolygon { get; }
            public double HalfSize { get; }
            public Vertex Center { get; }
        }

        private sealed class CellComparer : IComparer<Cell>
        {
            int IComparer<Cell>.Compare(Cell first, Cell second)
            {
                if (IsZero(first.MaxDistance - second.MaxDistance))
                {
                    return 0;
                }
                return first.MaxDistance.CompareTo(second.MaxDistance);
            }
        }
    }
}
