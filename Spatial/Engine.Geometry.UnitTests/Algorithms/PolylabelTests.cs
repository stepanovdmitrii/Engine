using NUnit.Framework;
using Engine.Geometry.Types;
using Engine.Geometry.Algorithms;
using System.Collections.Generic;
using System;

namespace Engine.Geometry.UnitTests.Algorithms
{
    public sealed class PolylabelTests
    {
        [Test]
        public void IsCorrectForSquare()
        {
            var square = new Polygon(new Vertex[] {
                new Vertex(0,0),
                new Vertex(100, 0),
                new Vertex(100, 100),
                new Vertex(0, 100)
            });

            Vertex result = Polylabel.ComputeLabelPosition(square);
            Assert.AreEqual(50, result.X, 1E-6, "Polylabel for square failed: {0}", nameof(result.X));
            Assert.AreEqual(50, result.Y, 1E-6, "Polylabel for square failed: {0}", nameof(result.Y)); 
        }

        [Test]
        public void IsCorrectForRectangle()
        {
            var rectangle = new Polygon(new Vertex[] {
                new Vertex(0,0),
                new Vertex(100, 0),
                new Vertex(100, 50),
                new Vertex(0, 50)
            });

            Vertex result = Polylabel.ComputeLabelPosition(rectangle);
            Assert.AreEqual(50, result.X, 1E-6, "Polylabel for rectangle failed: {0}", nameof(result.X));
            Assert.AreEqual(25, result.Y, 1E-6, "Polylabel for rectangle failed: {0}", nameof(result.Y));
        }

        [Test]
        public void IsCorrectForTriangle()
        {
            var triangle = new Polygon(new Vertex[] {
                new Vertex(0,0),
                new Vertex(100, 0),
                new Vertex(50, 100)
            });

            Vertex result = Polylabel.ComputeLabelPosition(triangle);
            Assert.AreEqual(50, result.X, 1E-6, "Polylabel for triangle failed: {0}", nameof(result.X));
            Assert.AreEqual(30.901700, result.Y, 1E-6, "Polylabel for triangle failed: {0}", nameof(result.Y));
        }

        [Test]
        public void ThrowsExceptionForIncompletePolygon()
        {
            var empty = new Polygon(new List<Vertex>());
            var onePoint = new Polygon(new[] { new Vertex(0, 0) });
            var twoPoints = new Polygon(new[] { new Vertex(0, 0), new Vertex(1, 1) });
            Assert.Throws<ArgumentOutOfRangeException>(() => Polylabel.ComputeLabelPosition(empty));
            Assert.Throws<ArgumentOutOfRangeException>(() => Polylabel.ComputeLabelPosition(onePoint));
            Assert.Throws<ArgumentOutOfRangeException>(() => Polylabel.ComputeLabelPosition(twoPoints));
        }
    }
}
